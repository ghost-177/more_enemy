using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoL.Core.Stations;
using LBoL.Presentation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SampleCharacterMod.Adventures
{
    // =====================================================================
    // Harmony 补丁集合：自定义事件运行时支持
    //
    // 架构说明（重要！）：
    //
    //   LBoL 事件的两个层次：
    //     Core 层：AdventureStation.OnEnter()  — 仅通知，不控制对话
    //     Presentation 层：GameMaster.AdventureFlow(Station) — 实际对话流程
    //
    //   GameMaster.AdventureFlow 是真正的入口，它负责：
    //     1. 查找 ExtraAdventureHandlers（官方自定义 handler 机制）
    //     2. 或启动 VnPanel.RunDialog 加载 YarnSpinner 对话
    //
    //   自定义 Mod 没有 .yarn 脚本文件，直接拦截 AdventureFlow，
    //   替换为自己的协程（发放奖励 → 让外层流程自动结束站点）。
    //
    // 补丁列表：
    //   1. Stage_Initialize_Patch
    //      - 每幕初始化后，向 AdventureConfig._IdTable 手动注册 config（关键！）
    //      - 将 SampleCustomEvent 注入 Stage.AdventurePool
    //
    //   2. GameMaster_AdventureFlow_Patch
    //      - 拦截 AdventureFlow，当当前 Adventure 是 SampleCustomEvent 时
    //      - 替换为自定义协程：随机发放奖励后 yield break
    //      - 外层 CoEnterStation 协程会在 AdventureFlow 结束后自动处理离站逻辑
    //
    //   3. SampleAdventureDebugHelper
    //      - 静态工具方法，供 F6 快捷键注入高权重事件（测试用）
    // =====================================================================

    // -----------------------------------------------------------------
    // 补丁 1：Stage.Initialize 后注入事件到 AdventurePool，
    //         并手动注册 AdventureConfig（sideloader 不会自动做这件事）
    // -----------------------------------------------------------------
    [HarmonyPatch]
    internal static class Stage_Initialize_Patch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod()
        {
            // Stage.Initialize 在 DoNotPublicize 列表中，须通过 AccessTools 定位
            return AccessTools.Method(typeof(Stage), "Initialize");
        }

        [HarmonyPostfix]
        static void Postfix(Stage __instance)
        {
            try
            {
                // 步骤 1：向 AdventureConfig._IdTable 注册 config
                //
                // 关键背景：
                //   AdventureConfig._IdTable 是 static Dictionary<string, AdventureConfig>
                //   Adventure.Initialize() 从这里按 Id 查 config，找不到就抛异常崩溃
                //   LBoLEntitySideloader 的 AdventureTemplate.Consume 不会自动写入这张表，
                //   必须手动注入，且要在游戏尝试创建 Adventure 实例之前完成。
                EnsureAdventureConfigRegistered();

                // 步骤 2：将自定义事件类型加入 AdventurePool
                //   实际出现概率由 SampleCustomEventWeighter.WeightFor() 控制
                var pool = __instance.AdventurePool;
                if (pool == null) return;

                pool.Add(typeof(SampleCustomEvent), 1f);
                BepinexPlugin.log.LogInfo("[SampleCustomEvent] 已加入 Stage.AdventurePool（第 " + __instance.Level + " 幕）");
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] Stage_Initialize_Patch 异常: " + e.Message);
            }
        }

        static bool _configRegistered = false;

        static void EnsureAdventureConfigRegistered()
        {
            if (_configRegistered) return;
            _configRegistered = true;

            try
            {
                var idTableField = AccessTools.Field(typeof(AdventureConfig), "_IdTable");
                var table = (Dictionary<string, AdventureConfig>)idTableField.GetValue(null);
                if (table == null)
                {
                    BepinexPlugin.log.LogWarning("[SampleCustomEvent] AdventureConfig._IdTable 为 null");
                    return;
                }

                const string eventId = "SampleCustomEvent";
                if (!table.ContainsKey(eventId))
                {
                    var config = new AdventureConfig(
                        No: 9001,        // 自定义编号，内置事件通常 < 100，使用大值避免冲突
                        Id: eventId,
                        HostId: "",      // NPC 角色 ID，留空=无 NPC
                        HostId2: "",
                        Music: 0,        // BGM，0=使用地图默认音乐
                        HideUlt: false,
                        TempArt: false
                    );
                    table[eventId] = config;
                    BepinexPlugin.log.LogInfo("[SampleCustomEvent] AdventureConfig 已注册到 _IdTable");
                }
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] 注册 AdventureConfig 失败: " + e.Message);
            }
        }
    }

    // -----------------------------------------------------------------
    // 补丁 2：拦截 GameMaster.AdventureFlow，替换为自定义事件协程
    //
    // 背景：
    //   AdventureStation.OnEnter() 是 Core 层通知，不控制对话流程。
    //   GameMaster.AdventureFlow(Station) 才是真正控制对话的方法：
    //     - 有 ExtraAdventureHandlers 时调用 handler（官方自定义机制）
    //     - 否则调用 VnPanel.RunDialog(adventure.DialogName, ...) 加载 Yarn 脚本
    //   自定义 Mod 没有 Yarn 脚本，直接拦截 AdventureFlow，
    //   用自定义协程取代，外层 CoEnterStation 会在协程结束后自动处理后续流程。
    // -----------------------------------------------------------------
    [HarmonyPatch]
    internal static class GameMaster_AdventureFlow_Patch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(GameMaster), "AdventureFlow");
        }

        [HarmonyPrefix]
        static bool Prefix(GameMaster __instance, Station station, ref IEnumerator __result)
        {
            // 只拦截 AdventureStation 且 Adventure 是自定义事件的情况
            if (!(station is AdventureStation advStation)) return true;
            if (!(advStation.Adventure is SampleCustomEvent customAdv)) return true;

            // 用自定义协程替换原始返回值
            __result = CustomAdventureFlow(advStation, customAdv);
            return false; // 跳过原始 AdventureFlow（不启动对话流程）
        }

        // 自定义事件流程协程
        //
        // 执行顺序（由外层 CoEnterStation 协程驱动）：
        //   CoEnterStation → AdventureFlow（我们接管） → 发放奖励 → yield break
        //   → CoEnterStation 继续 → EndStationFlow（游戏自动处理离站动画等）
        static IEnumerator CustomAdventureFlow(AdventureStation station, SampleCustomEvent adventure)
        {
            BepinexPlugin.log.LogInfo("[SampleCustomEvent] 「神秘旅行者的礼物」已触发！");

            try
            {
                // 随机发放三种奖励之一
                var rng = adventure.GameRun?.AdventureRng;
                float roll = rng != null
                    ? rng.NextFloat()
                    : (float)new System.Random().NextDouble();

                if (roll < 0.33f)
                {
                    // 奖励 A：金币
                    adventure.GainMoney(SampleCustomEvent.MoneyReward);
                    BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 奖励：{SampleCustomEvent.MoneyReward} 金币");
                }
                else if (roll < 0.66f)
                {
                    // 奖励 B：少量金币（卡牌升级的简化版，完整实现需要协程+UI）
                    adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
                    BepinexPlugin.log.LogInfo("[SampleCustomEvent] 奖励：40 金币（卡牌升级简化）");
                }
                else
                {
                    // 奖励 C：恢复 HP（最大 HP 的 HealPercent%）
                    var player = adventure.GameRun?.Player;
                    if (player != null)
                    {
                        int healAmt = Math.Max(1, player.MaxHp * SampleCustomEvent.HealPercent / 100);
                        adventure.Heal(healAmt, "SampleCustomEvent");
                        BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 奖励：恢复 {healAmt} HP");
                    }
                }
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] 奖励发放异常: " + e.Message);
            }

            // 协程结束，外层 CoEnterStation 会自动接管离站流程
            yield break;
        }
    }

    // =====================================================================
    // 调试辅助：强制将下一个 Adventure 节点设置为自定义事件
    //
    // 调用方式（在 BepinexPlugin.Update() 中通过快捷键触发）：
    //   SampleAdventureDebugHelper.ForceNextAdventure(gameRunController);
    //
    // 效果：以极高权重（9999f）向当前幕的 AdventurePool 注入自定义事件，
    //       使下一个事件节点必定触发。
    // =====================================================================
    internal static class SampleAdventureDebugHelper
    {
        public static void ForceNextAdventure(GameRunController gameRun)
        {
            if (gameRun == null)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] ForceNextAdventure: gameRun 为 null");
                return;
            }

            var stage = gameRun.CurrentStage;
            if (stage == null)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] ForceNextAdventure: CurrentStage 为 null");
                return;
            }

            var pool = stage.AdventurePool;
            if (pool != null)
            {
                pool.Add(typeof(SampleCustomEvent), 9999f);
                BepinexPlugin.log.LogInfo("[SampleCustomEvent] 已以高权重注入事件池，下一个事件节点将触发。");
                return;
            }
  
            BepinexPlugin.log.LogWarning("[SampleCustomEvent] ForceNextAdventure: AdventurePool 为 null");
        }
    }
}

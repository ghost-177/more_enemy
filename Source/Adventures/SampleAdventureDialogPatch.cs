using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoL.Core.Cards;
using LBoL.Core.Stations;
using LBoL.Presentation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SampleCharacterMod.Adventures
{
    // =====================================================================
    // Harmony 补丁集合：自定义事件运行时支持
    //
    // 补丁列表：
    //   1. Stage_Initialize_Patch
    //      - 手动注册 AdventureConfig 到 _IdTable（sideloader 不自动做）
    //      - 将 SampleCustomEvent 注入 Stage.AdventurePool
    //
    //   2. GameMaster_AdventureFlow_Patch
    //      - 拦截 AdventureFlow，替换为自定义协程
    //      - 通过 BepinexPlugin.OnGUI 展示：背景图（右半屏）+ 描述文 + 4 个选项按钮（左半屏）
    //      - 等待玩家点击后发放对应奖励：金币 / 治疗 / 展品 / 选卡
    //
    //   3. SampleAdventureDebugHelper
    //      - 强制注入高权重（F6 快捷键测试用）
    // =====================================================================

    // -----------------------------------------------------------------
    // 补丁 1：Stage.Initialize 后注入事件 + 注册 AdventureConfig
    // -----------------------------------------------------------------
    [HarmonyPatch]
    internal static class Stage_Initialize_Patch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod() =>
            AccessTools.Method(typeof(Stage), "Initialize");

        [HarmonyPostfix]
        static void Postfix(Stage __instance)
        {
            try
            {
                EnsureAdventureConfigRegistered();

                var pool = __instance.AdventurePool;
                if (pool == null) return;

                pool.Add(typeof(SampleCustomEvent), 1f);
                BepinexPlugin.log.LogInfo("[SampleCustomEvent] 已加入 Stage.AdventurePool（第 "
                    + __instance.Level + " 幕）");
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
                if (table == null) return;

                const string eventId = "SampleCustomEvent";
                if (!table.ContainsKey(eventId))
                {
                    table[eventId] = new AdventureConfig(
                        No: 9001, Id: eventId,
                        HostId: "", HostId2: "", Music: 0,
                        HideUlt: false, TempArt: false);
                    BepinexPlugin.log.LogInfo("[SampleCustomEvent] AdventureConfig 已注册");
                }
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] 注册 AdventureConfig 失败: " + e.Message);
            }
        }
    }

    // -----------------------------------------------------------------
    // 补丁 2：拦截 GameMaster.AdventureFlow，替换为自定义协程
    // -----------------------------------------------------------------
    [HarmonyPatch]
    internal static class GameMaster_AdventureFlow_Patch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod() =>
            AccessTools.Method(typeof(GameMaster), "AdventureFlow");

        [HarmonyPrefix]
        static bool Prefix(GameMaster __instance, Station station, ref IEnumerator __result)
        {
            if (!(station is AdventureStation advStation)) return true;
            if (!(advStation.Adventure is SampleCustomEvent customAdv)) return true;

            __result = CustomAdventureFlow(advStation, customAdv);
            return false;
        }

        // ----------------------------------------------------------------
        // 自定义事件主协程
        //
        // 流程：
        //   1. 加载背景图（嵌入 PNG）
        //   2. 设置描述文本 + 4 个选项，触发 BepinexPlugin.OnGUI 展示界面
        //      （界面布局：右半屏背景图，左半屏文字+按钮）
        //   3. 每帧 yield return null，等待玩家点击选项按钮
        //   4. 根据 pendingChoiceResult 发放对应奖励
        //   5. yield break → 外层 CoEnterStation 自动处理离站
        // ----------------------------------------------------------------
        static IEnumerator CustomAdventureFlow(AdventureStation station, SampleCustomEvent adventure)
        {
            BepinexPlugin.log.LogInfo("[SampleCustomEvent] 「神秘旅行者的礼物」已触发！");

            // --- 步骤 1：加载背景图 ---
            TryLoadBackground();

            // --- 步骤 2：从 YAML 读取事件描述 & 4 个主选项 ---
            const string eventId = "SampleCustomEvent";
            BepinexPlugin.pendingChoiceDescription =
                AdventureEventLocalize.GetDescription(eventId)
                ?? "一位神秘的旅行者出现在你面前。\n「旅行者，我有些东西想送给你——请慎重选择。」";

            BepinexPlugin.pendingChoiceOptions =
                AdventureEventLocalize.GetOptions(eventId)
                ?? new string[]
                {
                    $"【金币】接受钱袋（获得 {SampleCustomEvent.MoneyReward} 金币）",
                    $"【治疗】喝下治愈药水（恢复最大 HP 的 {SampleCustomEvent.HealPercent}%）",
                    "【展品】收下神秘遗物（随机获得一件展品）",
                    $"【卡牌】翻阅卡牌收藏（从 {SampleCustomEvent.CardOfferCount} 张随机卡中选 1 张）",
                };
            BepinexPlugin.pendingChoiceResult = -1;

            // --- 步骤 3：等待玩家选择 ---
            while (BepinexPlugin.pendingChoiceResult < 0)
                yield return null;

            int choice = BepinexPlugin.pendingChoiceResult;

            // 彻底清理主菜单 IMGUI 状态
            BepinexPlugin.pendingChoiceOptions = null;
            BepinexPlugin.pendingChoiceDescription = null;

            BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 玩家选择了选项 {choice}");

            // --- 步骤 4：发放奖励 ---
            switch (choice)
            {
                case 0: // 金币
                    adventure.GainMoney(SampleCustomEvent.MoneyReward);
                    BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 奖励：{SampleCustomEvent.MoneyReward} 金币");
                    break;

                case 1: // 治疗
                    int healPct = SampleCustomEvent.HealPercent;
                    adventure.HealPercentage(healPct);
                    BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 奖励：恢复 {healPct}% HP");
                    break;

                case 2: // 随机展品
                    foreach (var step in GainRandomExhibit(adventure))
                        yield return step;
                    break;

                case 3: // 选卡（从若干随机卡中选 1）
                    foreach (var step in SelectRandomCards(adventure))
                        yield return step;
                    break;
            }

            // 清理背景图
            BepinexPlugin.pendingChoiceBackground = null;

            // 必须手动调用 station.Finish() 解锁地图下一节点。
            // CoEnterStation 的自动处理依赖原生 AdventureFlow 内部的完成信号，
            // 我们完全替换了 AdventureFlow，因此需要自己触发。
            station.Finish();
            BepinexPlugin.log.LogInfo("[SampleCustomEvent] Station.Finish() 已调用，地图解锁。");
        }

        // ----------------------------------------------------------------
        // 奖励 C：随机展品
        //
        // 1. 调用 Stage.GetSpecialAdventureExhibit()（无参数，DoNotPublicize，用 AccessTools）
        //    → 返回一个 Exhibit 实例（已从展品池中移除）
        // 2. 通过 Adventure.GainExhibitRunner(exhibitId, message, optionIndex) 发放
        //    → 内建展品获取流程（IEnumerator），包含 UI 动画
        //
        // 注：Adventure.HealPercentage(int) 用于治疗，不在 DoNotPublicize 列表中
        // ----------------------------------------------------------------
        static IEnumerable GainRandomExhibit(SampleCustomEvent adventure)
        {
            // C# 规则：yield 不能出现在含 catch 子句的 try 块体中。
            // 解决方案：所有可能抛异常的代码先在 try-catch 里运行，
            // 然后 yield 在 try-catch 外面执行。

            LBoL.Core.Exhibit exhibit = null;
            IEnumerator coroutine = null;

            try
            {
                var stage = adventure.GameRun?.CurrentStage;
                if (stage != null)
                {
                    // GetSpecialAdventureExhibit 在 DoNotPublicize 列表中，需用 AccessTools
                    var rollMethod = AccessTools.Method(typeof(Stage), "GetSpecialAdventureExhibit");
                    exhibit = rollMethod?.Invoke(stage, null) as LBoL.Core.Exhibit;
                    if (exhibit != null)
                    {
                        BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 奖励展品：{exhibit.Id}");
                        // Adventure.GainExhibitRunner(string name, string message, int optionIndex)
                        coroutine = adventure.GainExhibitRunner(exhibit.Id, "", 0);
                    }
                }
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] GainRandomExhibit 准备异常: " + e.Message);
            }

            if (exhibit == null || coroutine == null)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] 无法获取展品，改发金币");
                adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
                yield break;
            } 

            // 驱动 GainExhibitRunner 协程
            // yield return 必须在 try-catch 外部，用标志变量传递信息
            bool keepDriving = true;
            while (keepDriving)
            {
                object current = null;
                bool moved = false;
                try
                {
                    keepDriving = coroutine.MoveNext();
                    if (keepDriving) { current = coroutine.Current; moved = true; }
                }
                catch (Exception e)
                {
                    BepinexPlugin.log.LogWarning("[SampleCustomEvent] GainExhibitRunner 执行异常: " + e.Message);
                    keepDriving = false;
                }
                // yield 在 try-catch 外执行（满足 C# 规则）
                if (moved) yield return current;
            }
        }

        // ----------------------------------------------------------------
        // 奖励 D：从若干随机卡中选 1 张加入牌库
        //
        // 1. 通过 AccessTools 调用 GameRunController.RollCards(rng, weightTable, count, ...)
        //    获取随机 Card 实例列表
        // 2. 用 IMGUI 展示卡牌名称按钮（第二阶段选择面板）
        // 3. 玩家选择后，调用 Adventure.GainCards(string[] names) 加入牌库
        //
        // 注：Adventure.SelectCards(string[]) 需要完整的 YarnSpinner dialog 上下文，
        //     我们已绕过 dialog，因此改用 IMGUI + GainCards 方式。
        // ----------------------------------------------------------------
        static IEnumerable SelectRandomCards(SampleCustomEvent adventure)
        {
            // 步骤 1：获取随机卡牌实例（全部在 try-catch 中，不含 yield）
            Card[] cards = null;
            try
            {
                var gameRun = adventure.GameRun;
                var stage = gameRun?.CurrentStage;
                if (gameRun != null && stage != null)
                {
                    // RollCards(RandomGen rng, CardWeightTable weightTable, Int32 count,
                    //           Boolean applyFactors, Boolean battleRolling, Predicate<CardConfig> filter)
                    // 用参数数量筛选，避免 typeof(CardWeightTable) 命名空间问题
                    var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    var rollCards = typeof(GameRunController)
                        .GetMethods(flags)
                        .FirstOrDefault(m => m.Name == "RollCards" && m.GetParameters().Length == 6);

                    if (rollCards != null)
                    {
                        cards = rollCards.Invoke(gameRun, new object[]
                        {
                            gameRun.CardRng,
                            stage.EnemyCardWeight,
                            SampleCustomEvent.CardOfferCount,
                            false,   // applyFactors
                            false,   // battleRolling
                            null     // filter（无限制）
                        }) as Card[];
                    }
                }
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] RollCards 失败: " + e.Message);
            }

            if (cards == null || cards.Length == 0)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] 无法获取卡牌，改发金币");
                adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
                yield break;
            }

            // 步骤 2：取得卡牌显示名称
            // GameEntity.Name 在 DoNotPublicize 列表中，需用 AccessTools
            var nameGetter = AccessTools.PropertyGetter(typeof(GameEntity), "Name");
            string[] cardLabels = cards.Select((c, i) =>
            {
                string name = null;
                try { name = nameGetter?.Invoke(c, null) as string; } catch { }
                return $"[{i + 1}] {name ?? c.Id}";
            }).ToArray();

            BepinexPlugin.log.LogInfo("[SampleCustomEvent] 提供卡牌：" + string.Join(", ", cardLabels));

            // 步骤 3：通过 IMGUI 展示卡牌选择面板（第二阶段）
            // yield 不在 try-catch 中，合法
            BepinexPlugin.pendingChoiceDescription =
                AdventureEventLocalize.GetCardSelectDescription("SampleCustomEvent")
                ?? "请从以下卡牌中选择一张加入牌库：";
            BepinexPlugin.pendingChoiceOptions = cardLabels;
            BepinexPlugin.pendingChoiceResult = -1;

            while (BepinexPlugin.pendingChoiceResult < 0)
                yield return null;

            int picked = BepinexPlugin.pendingChoiceResult;
            BepinexPlugin.pendingChoiceOptions = null;
            BepinexPlugin.pendingChoiceDescription = null;

            if (picked < 0 || picked >= cards.Length)
            {
                adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
                yield break;
            }

            // 步骤 4：Adventure.GainCards(string[] names) 加入选定卡牌
            // GainCards 是公开方法，直接调用（不需要 AccessTools）
            string selectedId = cards[picked].Id;
            BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 选择卡牌：{selectedId}");
            bool gainOk = false;
            try
            {
                adventure.GainCards(new string[] { selectedId });
                gainOk = true;
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] GainCards 失败: " + e.Message);
            }
            if (!gainOk)
                adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
        }

        // ----------------------------------------------------------------
        // 加载背景图（嵌入资源 Resources/Adventure/SampleCustomEventDef.png）
        //
        // 使用 Assembly.GetManifestResourceStream 直接加载，
        // 资源名称格式：{RootNamespace}.{文件夹}.{文件名}
        // ----------------------------------------------------------------
        static Texture2D _cachedBackground = null;

        static void TryLoadBackground()
        {
            if (_cachedBackground != null)
            {
                BepinexPlugin.pendingChoiceBackground = _cachedBackground;
                return;
            }

            try
            {
                var asm = System.Reflection.Assembly.GetExecutingAssembly();

                // 嵌入资源名称：RootNamespace.Resources.Adventure.SampleCustomEventDef.png
                // RootNamespace 由 csproj 设置（项目文件名去掉连字符）= SampleCharacterMod_windows
                const string resourceName = "SampleCharacterMod_windows.Resources.Adventure.SampleCustomEventDef.png";

                using (var stream = asm.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        if (tex.LoadImage(bytes))
                        {
                            _cachedBackground = tex;
                            BepinexPlugin.pendingChoiceBackground = _cachedBackground;
                            BepinexPlugin.log.LogInfo("[SampleCustomEvent] 背景图加载成功");
                        }
                        else
                        {
                            BepinexPlugin.log.LogWarning("[SampleCustomEvent] Texture2D.LoadImage 失败");
                        }
                    }
                    else
                    {
                        // 列出所有嵌入资源名称以便调试
                        var allNames = string.Join(", ", asm.GetManifestResourceNames());
                        BepinexPlugin.log.LogWarning($"[SampleCustomEvent] 未找到嵌入资源 '{resourceName}'，可用资源：{allNames}");
                    }
                }
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] 背景图加载失败: " + e.Message);
            }
        }
    }

    // =====================================================================
    // 调试辅助：强制下一个 Adventure 节点触发自定义事件（F6 快捷键）
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

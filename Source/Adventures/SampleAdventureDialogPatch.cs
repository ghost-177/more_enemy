using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoL.Core.Cards;
using LBoL.Core.Stations;
using LBoL.Presentation;
using LBoLEntitySideloader.Resource;
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
    //      - 通过 BepinexPlugin.OnGUI 展示：背景图 + 描述文 + 4 个选项按钮
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
        //   2. 设置描述文本 + 选项，触发 BepinexPlugin.OnGUI 展示界面
        //   3. 每帧 yield return null，等待玩家点击选项按钮
        //   4. 根据 pendingChoiceResult 发放对应奖励
        //   5. 清理状态，yield break → 外层 CoEnterStation 自动处理离站
        // ----------------------------------------------------------------
        static IEnumerator CustomAdventureFlow(AdventureStation station, SampleCustomEvent adventure)
        {
            BepinexPlugin.log.LogInfo("[SampleCustomEvent] 「神秘旅行者的礼物」已触发！");

            // --- 步骤 1：加载背景图 ---
            TryLoadBackground();

            // --- 步骤 2：准备事件描述 & 选项 ---
            BepinexPlugin.pendingChoiceDescription =
                "A mysterious traveler appears before you on the road.\n" +
                "\"Greetings, adventurer. I have something for you—choose wisely.\"";

            BepinexPlugin.pendingChoiceOptions = new string[]
            {
                $"[Gold]    Accept the coin purse  (+{SampleCustomEvent.MoneyReward} Gold)",
                $"[Heal]    Drink the healing potion  (Recover {SampleCustomEvent.HealPercent}% Max HP)",
                "[Exhibit] Receive a mysterious relic  (Gain a random Exhibit)",
                $"[Cards]   Browse the card collection  (Choose 1 of {SampleCustomEvent.CardOfferCount} Cards)",
            };
            BepinexPlugin.pendingChoiceResult = -1;

            // --- 步骤 3：等待玩家选择 ---
            while (BepinexPlugin.pendingChoiceResult < 0)
                yield return null;

            int choice = BepinexPlugin.pendingChoiceResult;

            // 彻底清理 IMGUI 状态
            BepinexPlugin.pendingChoiceOptions = null;
            BepinexPlugin.pendingChoiceDescription = null;
            BepinexPlugin.pendingChoiceBackground = null;

            BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 玩家选择了选项 {choice}");

            // --- 步骤 4：发放奖励 ---
            switch (choice)
            {
                case 0: // 金币
                    adventure.GainMoney(SampleCustomEvent.MoneyReward);
                    BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 奖励：{SampleCustomEvent.MoneyReward} 金币");
                    break;

                case 1: // 治疗
                    var player = adventure.GameRun?.Player;
                    if (player != null)
                    {
                        int healAmt = Math.Max(1, player.MaxHp * SampleCustomEvent.HealPercent / 100);
                        adventure.Heal(healAmt, "SampleCustomEvent");
                        BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 奖励：恢复 {healAmt} HP");
                    }
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

            // yield break → 外层 CoEnterStation 自动调用 EndStationFlow 处理离站
        }

        // ----------------------------------------------------------------
        // 奖励 C：随机展品
        //
        // 1. 通过 AccessTools 调用 Stage.RollExhibitInAdventure()
        //    （DoNotPublicize 列表，必须反射调用）
        // 2. 调用 GameRunController.TryCreateExhibit(Type) 创建展品实例
        // 3. 调用 GameRunController.GainExhibitInstantly(Exhibit) 立即获取
        //
        // Library（EnumerateRollableExhibitTypes）是虚属性被 Publicizer 排除，
        // 因此用 AccessTools 访问。
        // ----------------------------------------------------------------
        static IEnumerable GainRandomExhibit(SampleCustomEvent adventure)
        {
            Type exhibitType = null;

            // 优先用 Stage.RollExhibitInAdventure（DoNotPublicize）
            try
            {
                var stage = adventure.GameRun?.CurrentStage;
                if (stage != null)
                {
                    var rollMethod = AccessTools.Method(typeof(Stage), "RollExhibitInAdventure");
                    exhibitType = rollMethod?.Invoke(stage, null) as Type;
                }
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] RollExhibitInAdventure 失败: " + e.Message);
            }

            // 回退：从 Library 可滚展品中随机取一个
            if (exhibitType == null)
            {
                try
                {
                    var library = GetLibrary(adventure.GameRun);
                    if (library != null)
                    {
                        var enumMethod = AccessTools.Method(library.GetType(), "EnumerateRollableExhibitTypes");
                        var types = enumMethod?.Invoke(library, null) as IEnumerable<Type>;
                        exhibitType = types?.OrderBy(_ => UnityEngine.Random.Range(0, 10000)).FirstOrDefault();
                    }
                }
                catch (Exception e)
                {
                    BepinexPlugin.log.LogWarning("[SampleCustomEvent] EnumerateRollableExhibitTypes 失败: " + e.Message);
                }
            }

            if (exhibitType != null)
            {
                BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 奖励展品：{exhibitType.Name}");
                try
                {
                    // TryCreateExhibit 和 GainExhibitInstantly 均为虚方法，被 Publicizer 排除
                    // 必须通过 AccessTools 反射调用
                    var gameRun = adventure.GameRun;
                    var tryCreateExhibit = AccessTools.Method(gameRun.GetType(), "TryCreateExhibit");
                    var exhibit = tryCreateExhibit?.Invoke(gameRun, new object[] { exhibitType });
                    if (exhibit != null)
                    {
                        var gainInstantly = AccessTools.Method(gameRun.GetType(), "GainExhibitInstantly");
                        gainInstantly?.Invoke(gameRun, new object[] { exhibit });
                    }
                    else
                    {
                        BepinexPlugin.log.LogWarning("[SampleCustomEvent] TryCreateExhibit 返回 null");
                        adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
                    }
                }
                catch (Exception e)
                {
                    BepinexPlugin.log.LogWarning("[SampleCustomEvent] 获取展品异常: " + e.Message);
                    adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
                }
            }
            else
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] 无法获取展品，改发 40 金币");
                adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
            }

            yield break;
        }

        // ----------------------------------------------------------------
        // 奖励 D：从若干随机卡中选 1 张加入牌库
        //
        // 1. 通过 Library.EnumerateRollableCardTypes() 获取可滚卡牌类型
        //    （Library 是虚属性，用 AccessTools 访问）
        // 2. 随机取 CardOfferCount 张，调用 GameRunController.TryCreateCard 创建实例
        // 3. 调用 GameRunController.SelectCards(IEnumerable<Card>) 展示选卡界面
        //    （GameRunController 上的公有 SelectCards，带原生选卡 UI）
        // ----------------------------------------------------------------
        static IEnumerable SelectRandomCards(SampleCustomEvent adventure)
        {
            // 步骤 1：获取随机卡牌类型
            List<Type> cardTypes = null;
            try
            {
                var library = GetLibrary(adventure.GameRun);
                if (library != null)
                {
                    var enumMethod = AccessTools.Method(library.GetType(), "EnumerateRollableCardTypes");
                    var types = enumMethod?.Invoke(library, null) as IEnumerable<Type>;
                    cardTypes = types?
                        .OrderBy(_ => UnityEngine.Random.Range(0, 10000))
                        .Take(SampleCustomEvent.CardOfferCount)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] EnumerateRollableCardTypes 失败: " + e.Message);
            }

            if (cardTypes == null || cardTypes.Count == 0)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] 无法获取卡牌列表，改发 40 金币");
                adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
                yield break;
            }

            // 步骤 2：创建 Card 实例（TryCreateCard 是虚方法，被 Publicizer 排除，用 AccessTools）
            var gameRun2 = adventure.GameRun;
            var tryCreateCard = AccessTools.Method(gameRun2.GetType(), "TryCreateCard");
            var cards = new List<Card>();
            foreach (var type in cardTypes)
            {
                try
                {
                    var card = tryCreateCard?.Invoke(gameRun2, new object[] { type }) as Card;
                    if (card != null)
                        cards.Add(card);
                }
                catch (Exception e)
                {
                    BepinexPlugin.log.LogWarning($"[SampleCustomEvent] TryCreateCard({type.Name}) 失败: " + e.Message);
                }
            }

            if (cards.Count == 0)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] 没有成功创建任何卡牌，改发 40 金币");
                adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
                yield break;
            }

            BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 提供 {cards.Count} 张卡供选择");

            // 步骤 3：调用 GameRunController.SelectCards 展示原生选卡 UI
            // SelectCards 和 AddDeckCard 均为虚方法，用 AccessTools
            var gameRun3 = adventure.GameRun;
            var selectCardsMethod = AccessTools.Method(gameRun3.GetType(), "SelectCards");
            IEnumerator selectCoroutine = null;
            try
            {
                selectCoroutine = selectCardsMethod?.Invoke(gameRun3, new object[] { cards }) as IEnumerator;
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] SelectCards 调用失败: " + e.Message);
            }

            if (selectCoroutine != null)
            {
                while (true)
                {
                    bool hasNext = false;
                    try { hasNext = selectCoroutine.MoveNext(); }
                    catch (Exception e)
                    {
                        BepinexPlugin.log.LogWarning("[SampleCustomEvent] SelectCards 执行异常: " + e.Message);
                        break;
                    }
                    if (!hasNext) break;
                    yield return selectCoroutine.Current;
                }
            }
            else
            {
                // 如果 SelectCards 不是 IEnumerator，直接随机加一张卡作为退路
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] SelectCards 返回类型不兼容，随机加入一张卡");
                try
                {
                    var addDeckCard = AccessTools.Method(gameRun3.GetType(), "AddDeckCard");
                    addDeckCard?.Invoke(gameRun3, new object[] { cards[0] });
                }
                catch (Exception e)
                {
                    BepinexPlugin.log.LogWarning("[SampleCustomEvent] AddDeckCard 失败: " + e.Message);
                    adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
                }
            }
        }

        // ----------------------------------------------------------------
        // 辅助：获取 GameRunController.Library
        //
        // Library 是虚属性，被 Publicizer (IncludeVirtualMembers=false) 排除，
        // 必须通过 AccessTools 访问。
        // ----------------------------------------------------------------
        static object GetLibrary(GameRunController gameRun)
        {
            if (gameRun == null) return null;
            try
            {
                // 先尝试属性（虚属性在 publicized DLL 不可见，但 AccessTools 能找到）
                var prop = AccessTools.Property(typeof(GameRunController), "Library");
                if (prop != null) return prop.GetValue(gameRun);
            }
            catch { }
            try
            {
                // 再尝试字段（backing field 命名为 "library" 小写）
                var field = AccessTools.Field(typeof(GameRunController), "library");
                if (field != null) return field.GetValue(gameRun);
            }
            catch { }
            return null;
        }

        // ----------------------------------------------------------------
        // 加载背景图（嵌入资源 Resources/Adventure/SampleCustomEventDef.png）
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
                var sprite = ResourceLoader.LoadSprite(
                    "Adventure/SampleCustomEventDef.png",
                    BepinexPlugin.embeddedSource);

                if (sprite != null)
                {
                    _cachedBackground = sprite.texture;
                    BepinexPlugin.pendingChoiceBackground = _cachedBackground;
                    BepinexPlugin.log.LogInfo("[SampleCustomEvent] 背景图加载成功");
                }
                else
                {
                    BepinexPlugin.log.LogWarning("[SampleCustomEvent] 背景图 Sprite 为 null");
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

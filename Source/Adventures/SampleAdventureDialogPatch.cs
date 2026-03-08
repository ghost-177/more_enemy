using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Adventures;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Stations;
using LBoL.Presentation;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
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
    //      - 若本局游戏该事件尚未触发过，才将 SampleCustomEvent 注入 Stage.AdventurePool
    //      - 通过 GameRunController.ExtraFlags 检查"每局只出现一次"标记
    //
    //   2. GameMaster_AdventureFlow_Patch
    //      - 拦截 AdventureFlow，替换为自定义协程
    //      - 通过 BepinexPlugin.OnGUI 展示：背景图 + 描述文 + 4 个选项按钮
    //      - 等待玩家点击后发放对应奖励：金币 / 治疗 / 展品 / 选卡
    //      - 选卡奖励优先使用游戏原生 SelectCardPanel（同觉医生治疗事件的效果）
    //      - 事件完成后调用 adventure.SetGameRunFlag("SampleCustomEventCompleted") 防止重复出现
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

                // 检查本局游戏该事件是否已触发过
                // GameRunController.ExtraFlags 是 HashSet<string>，用于持久化标记
                var master = UnityEngine.Object.FindObjectOfType<GameMaster>();
                var gameRun = master?.CurrentGameRun;
                if (gameRun?.ExtraFlags?.Contains("SampleCustomEventCompleted") == true)
                {
                    BepinexPlugin.log.LogInfo("[SampleCustomEvent] 本局游戏已触发过，跳过注入。");
                    return;
                }

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
        //   3. 每帧 yield return null，等待玩家点击选项按钮
        //   4. 根据 pendingChoiceResult 发放对应奖励
        //   5. 标记事件已完成（防止本局再次出现）
        //   6. station.Finish() → 解锁地图下一节点
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

            // 清理主菜单 IMGUI 状态（选卡面板由游戏原生 UI 接管，此处仅清选项）
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
                    adventure.HealPercentage(SampleCustomEvent.HealPercent);
                    BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 奖励：恢复 {SampleCustomEvent.HealPercent}% HP");
                    break;

                case 2: // 随机展品
                    foreach (var step in GainRandomExhibit(adventure))
                        yield return step;
                    break;

                case 3: // 选卡（优先原生 SelectCardPanel，失败则 IMGUI 兜底）
                    foreach (var step in SelectRandomCards(adventure))
                        yield return step;
                    break;
            }

            // --- 步骤 5：标记本局游戏该事件已完成（防止重复出现）---
            // adventure.SetGameRunFlag 将字符串添加到 GameRunController.ExtraFlags（HashSet<string>）
            // 该字段会随存档持久化，因此即使存档后重进也不会重复出现
            try
            {
                adventure.SetGameRunFlag("SampleCustomEventCompleted");
                BepinexPlugin.log.LogInfo("[SampleCustomEvent] 事件完成标记已设置，本局不再重复出现。");
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] SetGameRunFlag 失败: " + e.Message);
            }

            // 清理背景图
            BepinexPlugin.pendingChoiceBackground = null;

            // 必须手动调用 station.Finish() 解锁地图下一节点
            station.Finish();
            BepinexPlugin.log.LogInfo("[SampleCustomEvent] Station.Finish() 已调用，地图解锁。");
        }

        // ----------------------------------------------------------------
        // 奖励 C：随机展品
        // ----------------------------------------------------------------
        static IEnumerable GainRandomExhibit(SampleCustomEvent adventure)
        {
            LBoL.Core.Exhibit exhibit = null;
            IEnumerator coroutine = null;

            try
            {
                var stage = adventure.GameRun?.CurrentStage;
                if (stage != null)
                {
                    var rollMethod = AccessTools.Method(typeof(Stage), "GetSpecialAdventureExhibit");
                    exhibit = rollMethod?.Invoke(stage, null) as LBoL.Core.Exhibit;
                    if (exhibit != null)
                    {
                        BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 奖励展品：{exhibit.Id}");
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
                if (moved) yield return current;
            }
        }

        // ----------------------------------------------------------------
        // 奖励 D：使用游戏原生 SelectCardPanel 选卡
        //
        // 流程（与 SampleCharacterDiscover 卡牌相同的方式）：
        //   1. 通过 RollCards 获取随机 Card 实例列表
        //   2. 直接用 Card[] 创建 MiniSelectCardInteraction(array, false, false, false)
        //      — 与 SampleCharacterDiscover 完全一致，卡牌详情可悬停查看
        //   3. 通过 UiManager.GetPanel<SelectCardPanel>() 获取面板
        //   4. 驱动 SelectCardPanel.ViewMiniSelect(interaction)
        //   5. 面板完成后，interaction.SelectedCard 即玩家选定的卡
        //   6. 调用 adventure.GainCards 将卡加入牌库
        //
        //   降级：若原生面板获取失败，退回 IMGUI 按钮选卡（兜底）
        // ----------------------------------------------------------------
        static IEnumerable SelectRandomCards(SampleCustomEvent adventure)
        {
            // --- 步骤 1：获取随机卡牌实例 ---
            Card[] cards = null;
            try
            {
                var gameRun = adventure.GameRun;
                var stage = gameRun?.CurrentStage;
                if (gameRun != null && stage != null)
                {
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

            // --- 步骤 2-4：直接创建 MiniSelectCardInteraction + 驱动原生面板 ---
            // 与 SampleCharacterDiscover 相同：new MiniSelectCardInteraction(array, false, false, false)
            // 不经过 adventure.SelectCards / DialogStorage，避免中间层重建卡实例
            MiniSelectCardInteraction interaction = null;
            IEnumerator viewCoroutine = null;
            bool useNativePanel = false;

            try
            {
                interaction = new MiniSelectCardInteraction(cards, false, false, false);

                var scPanel = UiManager.GetPanel<SelectCardPanel>();
                if (scPanel != null)
                {
                    viewCoroutine = scPanel.ViewMiniSelect(interaction);
                    useNativePanel = true;
                    BepinexPlugin.log.LogInfo("[SampleCustomEvent] 使用原生 SelectCardPanel 选卡");
                }
                else
                {
                    BepinexPlugin.log.LogWarning("[SampleCustomEvent] SelectCardPanel 未找到，退回 IMGUI");
                }
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning("[SampleCustomEvent] 原生选卡面板初始化失败: " + e.Message);
                useNativePanel = false;
            }

            if (useNativePanel && viewCoroutine != null)
            {
                // --- 步骤 4：驱动原生选卡面板协程 ---
                bool keepDriving = true;
                while (keepDriving)
                {
                    bool moved = false;
                    try
                    {
                        keepDriving = viewCoroutine.MoveNext();
                        if (keepDriving) moved = true;
                    }
                    catch (Exception e)
                    {
                        BepinexPlugin.log.LogWarning("[SampleCustomEvent] ViewMiniSelect 执行异常: " + e.Message);
                        keepDriving = false;
                    }
                    if (moved) yield return viewCoroutine.Current;
                }

                // --- 步骤 5-6：取结果并加入牌库 ---
                var selectedCard = interaction.SelectedCard;
                if (selectedCard != null)
                {
                    BepinexPlugin.log.LogInfo($"[SampleCustomEvent] 选定卡牌：{selectedCard.Id}");
                    bool ok = false;
                    try { adventure.GainCards(new[] { selectedCard.Id }); ok = true; }
                    catch (Exception e) { BepinexPlugin.log.LogWarning("[SampleCustomEvent] GainCards 失败: " + e.Message); }
                    if (!ok) adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
                }
                else
                {
                    BepinexPlugin.log.LogWarning("[SampleCustomEvent] 未选定卡牌，改发金币");
                    adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
                }
                yield break;
            }

            // --- 降级方案：IMGUI 按钮选卡（原生面板不可用时兜底）---
            BepinexPlugin.log.LogInfo("[SampleCustomEvent] 退回 IMGUI 选卡模式");
            foreach (var step in SelectRandomCardsImgui(adventure, cards))
                yield return step;
        }

        // ----------------------------------------------------------------
        // IMGUI 降级：用文字按钮列表让玩家选卡（原生面板不可用时）
        // ----------------------------------------------------------------
        static IEnumerable SelectRandomCardsImgui(SampleCustomEvent adventure, Card[] cards)
        {
            // 获取卡牌显示名称（GameEntity.Name 在 DoNotPublicize 列表中，用 AccessTools）
            var nameGetter = AccessTools.PropertyGetter(typeof(GameEntity), "Name");
            string[] cardLabels = cards.Select((c, i) =>
            {
                string name = null;
                try { name = nameGetter?.Invoke(c, null) as string; } catch { }
                return $"[{i + 1}] {name ?? c.Id}";
            }).ToArray();

            BepinexPlugin.log.LogInfo("[SampleCustomEvent] IMGUI 提供卡牌：" + string.Join(", ", cardLabels));

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

            string selectedId = cards[picked].Id;
            BepinexPlugin.log.LogInfo($"[SampleCustomEvent] IMGUI 选择卡牌：{selectedId}");
            bool gainOk = false;
            try { adventure.GainCards(new string[] { selectedId }); gainOk = true; }
            catch (Exception e) { BepinexPlugin.log.LogWarning("[SampleCustomEvent] GainCards 失败: " + e.Message); }
            if (!gainOk) adventure.GainMoney(SampleCustomEvent.MoneyReward / 2);
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
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
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

            // 如果事件已完成，先清除标记（方便调试反复触发）
            if (gameRun.ExtraFlags?.Contains("SampleCustomEventCompleted") == true)
            {
                gameRun.ExtraFlags.Remove("SampleCustomEventCompleted");
                BepinexPlugin.log.LogInfo("[SampleCustomEvent] 已清除完成标记（调试用）");
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

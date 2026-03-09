using LBoL.ConfigData;
using LBoL.Core.Adventures;
using LBoLEntitySideloader.Attributes;
using System;

namespace SampleCharacterMod.Adventures
{
    // =====================================================================
    // 示例事件 ——「神秘旅行者的礼物」
    //
    // 玩家在地图上经过事件节点（Adventure 节点）时触发此事件。
    //
    // 架构说明：
    //   LBoL 的事件系统分两个层次：
    //     1. 定义类（Def / Template）：SampleCustomEventDef
    //        - 继承 SampleAdventureTemplate（→ AdventureTemplate）
    //        - 负责提供 ID 和 AdventureConfig（NPC 形象、BGM 等）
    //     2. 逻辑类（Logic）：SampleCustomEvent
    //        - 继承 FakeAdventure（→ Adventure）
    //        - [EntityLogic] 将逻辑类与 Def 类绑定
    //        - [AdventureInfo] 提供权重控制器类型，用于控制出现概率
    //        - 事件触发后的效果由 SampleAdventureStation_Patch 处理
    //
    //   内置事件使用 YarnSpinner 对话脚本驱动，需要预编译的 .yarn 文件。
    //   自定义 Mod 无法方便地添加 Yarn 文件，因此本示例继承 FakeAdventure，
    //   并通过 Harmony 补丁（SampleAdventureStation_Patch）绕过对话阶段，
    //   直接触发奖励效果。
    //
    // 权重控制器：
    //   SampleCustomEventWeighter（嵌套类）实现 IAdventureWeighter，
    //   控制本事件在各幕中出现的概率。
    //   [AdventureInfo] 属性将逻辑类与权重控制器关联。
    //
    // 奖励池（由 SampleAdventureStation_Patch 随机挑选）：
    //   - 选项 A：获得金币（MoneyReward）
    //   - 选项 B：获得卡牌升级
    //   - 选项 C：恢复生命值
    // =====================================================================

    // -----------------------------------------------------------------
    // 1. 定义类（Def / Template）
    //    负责：ID、AdventureConfig（NPC、BGM 等外观设置）
    // -----------------------------------------------------------------
    public sealed class SampleCustomEventDef : SampleAdventureTemplate
    {
        public override AdventureConfig MakeConfig()
        {
            AdventureConfig config = GetDefaultAdventureConfig();

            // 主持人 NPC 的 ID（使用游戏内已有角色 ID）。
            // 留空时游戏使用默认/无角色外观。
            // 可填入如 "Rumia", "Mystia", "Mike" 等游戏内角色 ID。
            config.HostId = "Rumia";

            // 第二主持人（可选，留空即只有一位 NPC）
            config.HostId2 = "";

            // 背景音乐：游戏使用整数 ID 指定 BGM。0 = 使用地图默认音乐。
            config.Music = 0;

            // 是否在事件中隐藏大招槽位
            config.HideUlt = false;

            return config;
        }
    }

    // -----------------------------------------------------------------
    // 2. 逻辑类（Logic）
    //    [EntityLogic]    将此类与上方 Def 类绑定
    //    [AdventureInfo]  注册权重控制器，控制本事件的出现概率
    //
    //    继承 Adventure（非 FakeAdventure，FakeAdventure 是 sealed 无法继承）
    //    实际流程由 GameMaster_AdventureFlow_Patch 完全接管：
    //      - 拦截 AdventureFlow，跳过 YarnSpinner 对话
    //      - 通过 IMGUI 展示选项面板（背景图 + 描述 + 4 个按钮）
    //      - 等待玩家点击后发放对应奖励
    //      - 选卡奖励使用游戏原生 SelectCardPanel（同觉医生治疗事件）
    //      - 每局游戏只出现一次（通过 GameRunController.ExtraFlags 标记）
    // -----------------------------------------------------------------
    [AdventureInfo(WeighterType = typeof(SampleCustomEventWeighter))]
    [EntityLogic(typeof(SampleCustomEventDef))]
    public sealed class SampleCustomEvent : Adventure
    {
        // 奖励参数
        public const int MoneyReward    = 800;   // 选项 1：获得金币数量
        public const int HealPercent    = 25;    // 选项 2：恢复最大 HP 的百分比
        // 选项 3：随机展品（由 GetSpecialAdventureExhibit 决定）
        public const int CardOfferCount = 5;     // 选项 4：供选择的卡牌张数（原生面板可展示较多张）

        // -----------------------------------------------------------------
        // 嵌套权重控制器
        //
        // 实现 IAdventureWeighter 接口，控制本事件在地图生成时的出现概率。
        // - WeightFor 返回 > 0：事件加入随机池（值越大越常见）
        // - WeightFor 返回   0：当前条件下不出现
        //
        // 游戏在每次幕初始化（Stage.Initialize）后将所有已注册事件类型
        // 加入事件池，并使用各事件的权重控制器调整最终权重。
        // -----------------------------------------------------------------
        public sealed class SampleCustomEventWeighter : IAdventureWeighter
        {
            public float WeightFor(Type adventureType, LBoL.Core.GameRunController gameRun)
            {
                // 仅处理本事件；其他类型不影响
                if (adventureType != typeof(SampleCustomEvent))
                    return 0f;

                // 本事件在第 1、2 幕出现，权重为 1（与同幕其他事件等概率）
                // 调试时可临时改为较大值（如 100f）使事件更频繁出现
                int stageLevel = gameRun.CurrentStage?.Level ?? 0;
                if (stageLevel == 1 || stageLevel == 2)
                    return 1f;

                return 0f;
            }
        }
    }
}

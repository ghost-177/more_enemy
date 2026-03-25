using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.StatusEffects.Template;
using UnityEngine;

namespace EternalWinterMod.Enemies.Act1
{
    // ============================================================
    // 「冻结领域」— 蕾蒂永久被动SE（挂在蕾蒂自身上）
    // 效果：每当玩家回合开始时，玩家失去 Level 点法力（冻结）
    // Level = 战斗开始时由蕾蒂的 Power 字段决定
    // ============================================================

    public sealed class LettyFreezeAuraSEDef : EternalWinterSeTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Type = StatusEffectType.Special;
            cfg.HasLevel = true;
            cfg.LevelStackType = StackType.Keep;
            cfg.IsStackable = false;
            cfg.HasDuration = false;
            cfg.IsVerbose = false;
            return cfg;
        }

        // 暂无专属图标，返回 null 由游戏显示占位图
        public override Sprite LoadSprite() => null;
    }

    [EntityLogic(typeof(LettyFreezeAuraSEDef))]
    public sealed class LettyFreezeAuraSE : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            // 监听玩家回合开始事件（而非 Owner 的回合）
            // ReactOwnerEvent 支持任意 GameEvent，不限于 Owner 本身
            ReactOwnerEvent(Battle.Player.TurnStarted, OnPlayerTurnStarted);
        }

        private IEnumerable<BattleAction> OnPlayerTurnStarted(UnitEventArgs args)
        {
            if (Owner != null && Level > 0)
            {
                // 玩家失去 Level 点随机法力（冻结效果）
                yield return new LoseTurnManaAction(new ManaGroup() { Any = Level });
            }
        }
    }
}

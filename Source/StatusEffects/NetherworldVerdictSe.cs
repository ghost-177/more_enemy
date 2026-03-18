using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.StatusEffects.Template;

namespace EternalWinterMod.StatusEffects
{
    // ============================================================
    // 冥判书 持有效果：每回合结束时失去 Level×2 HP（穿透格挡）
    // 由冥判书卡牌在 OnDraw 时施加到玩家身上，
    // 在 OnLeaveHand 时移除。
    // ============================================================

    public sealed class NetherworldVerdictSeDef : EternalWinterSeTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Type = StatusEffectType.Negative;
            cfg.HasLevel = true;
            cfg.LevelStackType = StackType.Add;
            cfg.IsStackable = true;
            return cfg;
        }
    }

    [EntityLogic(typeof(NetherworldVerdictSeDef))]
    public sealed class NetherworldVerdictSe : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(Owner.TurnEnded, OnOwnerTurnEnded);
        }

        private IEnumerable<BattleAction> OnOwnerTurnEnded(UnitEventArgs args)
        {
            if (Owner != null)
            {
                // 每层 2 HP 穿透伤害（冥判书持有惩罚）
                yield return new DamageAction(Owner, Owner, DamageInfo.HpLose(Level * 2f, false), "", GunType.Single);
            }
        }
    }
}

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

namespace EternalWinterMod.StatusEffects
{
    // ============================================================
    // 幽冥令 持有效果：每打出1张牌，额外失去 Level 点法力
    // 近似实现"所有牌费用+1"——每出一张牌额外扣除1点法力
    // ============================================================

    public sealed class NetherworldDecreeSeDef : EternalWinterSeTemplate
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

    [EntityLogic(typeof(NetherworldDecreeSeDef))]
    public sealed class NetherworldDecreeSe : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(Battle.CardUsed, OnCardUsed);
        }

        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            if (Owner != null && args.Card != null)
            {
                // 每张出牌额外失去 Level 点法力
                yield return new LoseTurnManaAction(new ManaGroup() { Any = Level });
            }
        }
    }
}

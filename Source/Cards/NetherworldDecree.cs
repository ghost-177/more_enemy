using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Cards.Template;
using EternalWinterMod.StatusEffects;
using LBoL.Core;
using MadokaMod.Cards.Template;

namespace EternalWinterMod.Cards
{
    // 幽冥令：持有时每出一张牌失去1点法力（via NetherworldDecreeSe）
    public sealed class NetherworldDecreeDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = CardIndexGenerator.GetUniqueIndex(cfg);
            cfg.RelativeEffects = new System.Collections.Generic.List<string>
                { nameof(NetherworldDecreeSe) };
            return cfg;
        }
    }

    [EntityLogic(typeof(NetherworldDecreeDef))]
    public sealed class NetherworldDecree : Card
    {
        public override IEnumerable<BattleAction> OnDraw()
        {
            yield return new ApplyStatusEffectAction(
                typeof(NetherworldDecreeSe), Battle.Player, 1, null, null, null, 0f, false);
        }

        public override void OnLeaveHand()
        {
            RemoveDecreeSe();
        }

        private void RemoveDecreeSe()
        {
            var se = Battle.Player.GetStatusEffect<NetherworldDecreeSe>();
            if (se != null && se.Level > 0)
                React(new RemoveStatusEffectAction(se, false, 0f));
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
        }
    }
}

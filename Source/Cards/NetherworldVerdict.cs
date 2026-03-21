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

namespace EternalWinterMod.Cards
{
    // 冥判书：持有时每回合结束受2穿透伤害（via NetherworldVerdictSe）
    public sealed class NetherworldVerdictDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = 30005;
            cfg.RelativeEffects = new System.Collections.Generic.List<string>
                { nameof(NetherworldVerdictSe) };
            return cfg;
        }
    }

    [EntityLogic(typeof(NetherworldVerdictDef))]
    public sealed class NetherworldVerdict : Card
    {
        public override IEnumerable<BattleAction> OnDraw()
        {
            yield return new ApplyStatusEffectAction(
                typeof(NetherworldVerdictSe), Battle.Player, 1, null, null, null, 0f, false);
        }

        public override void OnLeaveHand()
        {
            RemoveVerdictSe();
        }

        private void RemoveVerdictSe()
        {
            var se = Battle.Player.GetStatusEffect<NetherworldVerdictSe>();
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

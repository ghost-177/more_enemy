using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Cards.Template;
using LBoL.Core;
using MadokaMod.Cards.Template;

namespace EternalWinterMod.Cards
{
    // 号外报道：抽到时受4穿透伤害，然后放逐自身
    public sealed class ExtraNewsReportDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = CardIndexGenerator.GetUniqueIndex(cfg);
            cfg.Value1 = 4;
            return cfg;
        }
    }

    [EntityLogic(typeof(ExtraNewsReportDef))]
    public sealed class ExtraNewsReport : Card
    {
        public override IEnumerable<BattleAction> OnDraw()
        {
            yield return new DamageAction(
                Battle.Player, Battle.Player,
                DamageInfo.HpLose(Value1, false),
                "", GunType.Single);
            yield return new ExileCardAction(this);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
        }
    }
}

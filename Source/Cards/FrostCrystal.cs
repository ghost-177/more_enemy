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
    // 寒冰晶：抽到时消耗1点当前法力
    public sealed class FrostCrystalDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = CardIndexGenerator.GetUniqueIndex(cfg);
            cfg.Keywords = Keyword.Exile;
            return cfg;
        }
    }

    [EntityLogic(typeof(FrostCrystalDef))]
    public sealed class FrostCrystal : Card
    {
        public override IEnumerable<BattleAction> OnDraw()
        {
            yield return new LoseTurnManaAction(new ManaGroup() { Any = 1 });
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
        }
    }
}

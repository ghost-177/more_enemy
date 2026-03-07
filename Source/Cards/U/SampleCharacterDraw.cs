using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Battle.BattleActions;
using LBoL.EntityLib.StatusEffects.Neutral.Blue;
using LBoL.EntityLib.StatusEffects.Cirno;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterDrawDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Any = 1, Blue = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Nobody;

            config.Value1 = 2;
            config.UpgradedValue1 = 3;

            config.Value2 = 3;
            config.UpgradedValue2 = 4;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterDrawDef))]
    public sealed class SampleCharacterDraw : SampleCharacterCard
    {
        //Using custom Value3 defined in SampleCharacterCard for display purposes.
        protected override int BaseValue3 {get; set; } = 1;
        protected override int BaseUpgradedValue3 {get; set; } = 1; 
        
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return new DrawManyCardAction(base.Value1);
            //Draw X cards at the start of the next turn.
            yield return new ApplyStatusEffectAction<ExtraDraw>(Battle.Player, base.Value2, 0, 0, 0, 0.2f);
            //At the start of each turn, draw X additional card.
            yield return new ApplyStatusEffectAction<MoreDraw>(Battle.Player, Value3, 0, 0, 0, 0.2f);
            yield break;
        }
    }
}



using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterBlockBDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.IsPooled = false;

            config.Colors = new List<ManaColor>() { ManaColor.Black };
            config.Cost = new ManaGroup() { Any = 1, Black = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 2};
            config.Rarity = Rarity.Common;

            config.Type = CardType.Defense;
            config.TargetType = TargetType.Self;

            config.Block = 10;
            config.UpgradedBlock = 13;

            //The card is treated as a basic card. 
            config.Keywords = Keyword.Basic;
            config.UpgradedKeywords = Keyword.Basic;

            //Keyword that doesn't add an effect to the card, but to add to the card's tooltips.
            config.RelativeKeyword = Keyword.Block;
            config.UpgradedRelativeKeyword = Keyword.Block;

            config.Illustrator = "Cool_illustrator";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterBlockBDef))]
    public sealed class SampleCharacterBlockB : SampleCharacterCard
    {
        //By default, if config.Damage / config.Block / config.Shield are set:
        //The card will deal damage or gain Block/Barrier without having to set anything.
        //Here, this is is equivalent to the following code.
         
        /*protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return DefenseAction(true); 
        }*/
    }
}



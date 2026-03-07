using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Battle.BattleActions;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterCardGenerationDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.Colors = new List<ManaColor>() { ManaColor.White };
            config.Cost = new ManaGroup() { White = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Nobody;

            config.Value1 = 1;
            config.UpgradedValue1 = 2;

            config.Illustrator = "";

            config.RelativeCards = new List<string>() { nameof(SampleCharacterToken) };
            config.UpgradedRelativeCards = new List<string>() { nameof(SampleCharacterToken) };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterCardGenerationDef))]
    public sealed class SampleCharacterCardGeneration : SampleCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            //Add a token card to the hand.
            yield return new AddCardsToHandAction(Library.CreateCards<SampleCharacterToken>(Value1, false));
            //Add a token card to the draw pile in a random position.
            yield return new AddCardsToDrawZoneAction(Library.CreateCards<SampleCharacterToken>(base.Value1, false), DrawZoneTarget.Random, AddCardsType.Normal);
            //Add a token card to the discard pile.
            yield return new AddCardsToDiscardAction(Library.CreateCards<SampleCharacterToken>(base.Value1, false), AddCardsType.Normal);
            yield break;
        }
    }
}



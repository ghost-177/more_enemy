using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Battle.BattleActions;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterDiscoverDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
        
            config.Colors = new List<ManaColor>() { ManaColor.White };
            config.Cost = new ManaGroup() { Any = 1, White = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 3;
            config.UpgradedValue1 = 5;

            config.Value2 = 1;

            config.Mana = new ManaGroup() { Any = 0 };

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterDiscoverDef))]
    public sealed class SampleCharacterDiscover : SampleCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            Card[] array = base.Battle.RollCardsWithoutManaLimit(
                new CardWeightTable
                (
                    RarityWeightTable.BattleCard, //Rarity distribution of the cards. (BattleCard: 40% Common, 40% Uncommon, 20% Rare)
                    OwnerWeightTable.Valid, //Player and Neutral card pool. (Valid: Includes the Character, Act 1 Boss Exhibit and Neutral cards.) 
                    CardTypeWeightTable.CanBeLoot, //Card types. (Can Be Loot: Can be Attack, Defense, Skill, ability; Cannot be Tools).
                    false
                ),
                base.Value1, //Total amount of card to choose from.
                (CardConfig config) => config.Id != base.Id && config.Keywords.HasFlag(Keyword.Accuracy)); 
            //Card selection.
            MiniSelectCardInteraction interaction = new MiniSelectCardInteraction(array, false, false, false)
            {
				Source = this
			};        
			yield return new InteractionAction(interaction, false);
			Card selectedCard = interaction.SelectedCard;
            //Change the cost of the card.
			selectedCard.SetTurnCost(base.Mana);
			selectedCard.IsEthereal = true;
			selectedCard.IsExile = true;
			yield return new AddCardsToHandAction(new Card[] { selectedCard });

            //To choose more than 1 card.
			/*Interaction interactionMultiple = new SelectCardInteraction(0, base.Value2, array, 0);
            IReadOnlyList<Card> cards = ((SelectCardInteraction)interactionMultiple).SelectedCards;
            yield return new AddCardsToHandAction(cards);*/
            
			yield break;
        }
    }
}



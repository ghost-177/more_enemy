using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;

namespace SampleCharacterMod.Cards.B
{
    public sealed class SampleCharacterTeammateDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Blue, ManaColor.Green };
            config.Cost = new ManaGroup() { Blue = 1, Green = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Friend;
            config.TargetType = TargetType.Nobody;

            //Loyalty is called "Unity" ingame.
            config.Loyalty = 4;
            config.UpgradedLoyalty = 4;
            //Passive cost is the passive amount of Unity gained/consumed at the strt of each turn.  
            config.PassiveCost = 1;
            config.UpgradedPassiveCost = 2;
            //Cost of the Active ability. 
            config.ActiveCost = -4;
            config.UpgradedActiveCost = -3;
            //Cost of the Ultimate ability.
            config.UltimateCost = -8;
            config.UpgradedUltimateCost = -6;

            config.Value1 = 1;
            config.Value2 = 3;
            
            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            
            return config;
        }
    }

    [EntityLogic(typeof(SampleCharacterTeammateDef))]
    public sealed class SampleCharacterTeammate : SampleCharacterCard
    {
        public string Indent {get;} = "<indent=80>";
        public string PassiveCostIcon
        {
            get
            {
                return string.Format("<indent=0><sprite=\"Passive\" name=\"{0}\">{1}", base.PassiveCost, Indent);
            }
        }
        public string ActiveCostIcon
        {
            get
            {
                return string.Format("<indent=0><sprite=\"Active\" name=\"{0}\">{1}", base.ActiveCost, Indent);
            }
        }
        public string UltimateCostIcon
        {
            get
            {
                return string.Format("<indent=0><sprite=\"Ultimate\" name=\"{0}\">{1}", base.UltimateCost, Indent);
            }
        }

        //Effect to trigger at the start of the end.
        public override IEnumerable<BattleAction> OnTurnStartedInHand()
		{
			return this.GetPassiveActions();
		}

        public override IEnumerable<BattleAction> GetPassiveActions()
		{
            //Triigger the effect only if the card has been summoned. 
			if (!base.Summoned || base.Battle.BattleShouldEnd)
			{
				yield break;
			}
			base.NotifyActivating();
            //Increase base loyalty.
			base.Loyalty += base.PassiveCost;
			int num;
            //Trigger the action multiple times if "Mental Energy Injection" is active.
			for (int i = 0; i < base.Battle.FriendPassiveTimes; i = num + 1)
			{
				if (base.Battle.BattleShouldEnd)
				{
					yield break;
				}
                yield return new DrawManyCardAction(base.Value1);
				num = i;
			}
			yield break;
		}

        //Action to perform when the teammate card is summoned.
        protected override IEnumerable<BattleAction> SummonActions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
		{
            yield return new DrawManyCardAction(base.Value1);
            foreach (BattleAction battleAction in base.SummonActions(selector, consumingMana, precondition))
			{
				yield return battleAction;
			}

            yield break;
		}

        //When the summoned card is played, choose and resolve either the active or ultimate effect.
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
		{
            //
			if (precondition == null || ((MiniSelectCardInteraction)precondition).SelectedCard.FriendToken == FriendToken.Active)
			{
                //Adjust the card's loyalty. 
                //Because the ActiveCost is negative, the Cost has to be added instead of substracted.
				base.Loyalty += base.ActiveCost;
                yield return new DrawManyCardAction(base.Value1);
				yield return base.SkillAnime;
			}
			else
			{
				base.Loyalty += base.UltimateCost;
                base.UltimateUsed = true;
				yield return new DrawManyCardAction(base.Value2);
                yield return base.SkillAnime;
			}
			yield break;
		}
    }
}



using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Battle.BattleActions;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterUnplayableDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Black };
            config.Cost = new ManaGroup() { };
            config.UpgradedCost = new ManaGroup() { };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Nobody;

            config.Value1 = 3;
            config.UpgradedValue1 = 5;

            //The Forbidden Keyword makes the card unplayable. 
            //The Shield keyword is here to add the Barrier description to the tooltip. 
            config.Keywords = Keyword.Retain | Keyword.Forbidden | Keyword.Shield;
            config.UpgradedKeywords = Keyword.Retain | Keyword.Forbidden | Keyword.Shield;
            
            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterUnplayableDef))]
    public sealed class SampleCharacterUnplayable : SampleCharacterCard
    {
        //While in combat, react to the Player's turn ending.
        protected override void OnEnterBattle(BattleController battle)
        {
            base.ReactBattleEvent<UnitEventArgs>(base.Battle.Player.TurnEnded, this.OnPlayerTurnEnded);
        }

        private IEnumerable<BattleAction> OnPlayerTurnEnded(UnitEventArgs args)
        {
            //Only apply this effect if the card is in the hand.
            if (this.Zone == CardZone.Hand)
            {
                base.NotifyActivating();

                //Gain a fix amount of barrier (unaffected by spirit).
                yield return new CastBlockShieldAction(base.Battle.Player, base.Battle.Player, 0, base.Value1, BlockShieldType.Direct, true);
            }
            yield break;
        }
    }
}



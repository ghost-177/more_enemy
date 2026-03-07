using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using SampleCharacterMod.GunName;
using LBoL.Core.Battle;
using LBoL.Core;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterDynamicCostDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.GunName = GunNameID.GetGunFromId(400);

            config.Colors = new List<ManaColor>() { ManaColor.Black};
            config.Cost = new ManaGroup() { Any = 9, Black = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 8 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.SingleEnemy;

            config.Damage = 50;
            config.UpgradedDamage = 60;

            //config.Mana is used here to set 
            config.Mana = new ManaGroup { Any = 1 };

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterDynamicCostDef))]
    public sealed class SampleCharacterDynamicCost : SampleCharacterCard
    {
        //Cost -1 less for each card in hand.
        protected override ManaGroup AdditionalCost
		{
			get
			{
                //Only change the cost while in Battle
                if (base.Battle != null)
                {
                    return base.Mana * - base.Battle.HandZone.Count;
                }
                //Otherwise, do not change the ccost (0 additional cost)
                return base.Mana * - 0;
            }
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return base.AttackAction(selector, base.GunName);
            yield break;
        }
    }
}



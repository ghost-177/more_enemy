using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using SampleCharacterMod.GunName;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterMultiAttackDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.GunName = GunNameID.GetGunFromId(400);

            config.Colors = new List<ManaColor>() { ManaColor.Red };
            config.Cost = new ManaGroup() { Any = 1, Red = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 2 };
            config.Rarity = Rarity.Common;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.SingleEnemy;

            config.Damage = 8;
            config.UpgradedDamage = 10;

            config.Value1 = 2;
            config.UpgradedValue1 = 3;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterMultiAttackDef))]
    public sealed class SampleCharacterMultiAttack : SampleCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            //Attack the enemy Value1 times. The attack animation is only played once.
            //To play the animation as many time as Value1, set the last parameter of Guns() from false to true.
            base.CardGuns = new Guns(base.GunName, base.Value1, false);
			foreach (GunPair gunPair in base.CardGuns.GunPairs)
			{
				yield return base.AttackAction(selector, gunPair);
			}
        }
    }
}



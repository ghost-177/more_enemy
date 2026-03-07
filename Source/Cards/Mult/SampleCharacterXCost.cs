using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using SampleCharacterMod.GunName;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Battle.BattleActions;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterXCostDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.GunName = GunNameID.GetGunFromId(400);

            config.Colors = new List<ManaColor>() { ManaColor.Blue, ManaColor.Red };
            //Mana to consume aside from X
            config.Cost = new ManaGroup() { Blue = 1, Red = 1 };
            config.Rarity = Rarity.Rare;

            //The XCost has to be set.
            config.IsXCost = true;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.SingleEnemy;

            config.Damage = 5;
            config.UpgradedDamage = 8;

            config.Value1 = 5;
            config.UpgradedValue1 = 7;

            config.Value2 = 2;

            config.Mana = new ManaGroup() { Red = 1 };

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterXCostDef))]
    public sealed class SampleCharacterXCost : SampleCharacterCard
    {
        //Create another ManaGroup that can be used to display the UU mana on the card.
        private ManaGroup _blueMana = new ManaGroup() { Blue = 2 };  
        public ManaGroup BlueMana
        {
            get
            {
                return _blueMana;
            }
        }

        //Get the amount of mana that was pooled for the X-cost card.
        public override ManaGroup GetXCostFromPooled(ManaGroup pooledMana)
		{
			return new ManaGroup
			{
				Blue = pooledMana.Blue,
				Red = pooledMana.Red,
				Philosophy = pooledMana.Philosophy
			};
		}
        
        //Calcualte the amount of damage dealt based on the amount of Red mana used.
        //Outside of combat, displays the base amount of damage.
        private DamageInfo CalculateDamage(ManaGroup? manaGroup)
		{
			if (manaGroup != null)
			{
				ManaGroup valueOrDefault = manaGroup.GetValueOrDefault();
				return DamageInfo.Attack((float)(base.RawDamage + base.SynergyAmount(valueOrDefault, ManaColor.Red, 1) * base.Value1), base.IsAccuracy);
			}
			return DamageInfo.Attack((float)base.RawDamage, base.IsAccuracy);
		}

		public override DamageInfo Damage
		{
			get
			{
				return this.CalculateDamage(base.PendingManaUsage);
			}
		}

    
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
		{
            //Red Mana: Deal damage.
			yield return base.AttackAction(selector, this.CalculateDamage(new ManaGroup?(consumingMana)), null);

            //Blue mana: Gain life.
            int blueMana = base.SynergyAmount(consumingMana, ManaColor.Blue, 2);
            yield return new HealAction(base.Battle.Player, base.Battle.Player, blueMana * base.Value2);
            yield break;
		}
    }
}



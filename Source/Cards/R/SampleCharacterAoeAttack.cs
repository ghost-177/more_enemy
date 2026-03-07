using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using SampleCharacterMod.GunName;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.StatusEffects;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterAoeAttackDef : SampleCharacterCardTemplate
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
            //TargetType.AllEnemies will change the selector to all enemies for attacks/status effects.
            config.TargetType = TargetType.AllEnemies;

            config.Damage = 13;
            config.UpgradedDamage = 15;

            config.Value1 = 2;
            config.UpgradedValue1 = 3;

            //Add Lock On descrption when hovering over the card.
            config.RelativeEffects = new List<string>() { nameof(LockedOn) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(LockedOn) };

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterAoeAttackDef))]
    public sealed class SampleCharacterAoeAttack : SampleCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            //Attack all enemies, selector is set to Battle.AllAliveEnemies.
            yield return base.AttackAction(selector, base.GunName);
            //If the battle were to end, skip the rest of the method.
            if (base.Battle.BattleShouldEnd)
            {
                yield break;
            }
            //Apply the Lock On Debuff to all alive enemies.
            foreach (BattleAction battleAction in base.DebuffAction<LockedOn>(base.Battle.AllAliveEnemies, base.Value1, 0, 0, 0, true, 0.2f))
            {
                yield return battleAction;
            }
            yield break;
        }
    }
}



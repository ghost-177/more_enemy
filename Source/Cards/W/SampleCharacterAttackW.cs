using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using SampleCharacterMod.GunName;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterAttackWDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.GunName = GunNameID.GetGunFromId(400);
            config.IsPooled = false;

            config.Colors = new List<ManaColor>() { ManaColor.White };
            config.Cost = new ManaGroup() { Any = 1, White = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 2 };
            config.Rarity = Rarity.Common;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.SingleEnemy;

            config.Damage = 10;
            config.UpgradedDamage = 14;

            config.Keywords = Keyword.Basic;
            config.UpgradedKeywords = Keyword.Basic;

            config.Illustrator = "Wholesome_illustrator";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterAttackWDef))]
    public sealed class SampleCharacterAttackW : SampleCharacterCard
    {
        //By default, if config.Damage / config.Block / config.Shield are set:
        //The card will deal damage or gain Block/Barrier without having to set anything.
        //Here, this is is equivalent to the following code.
         
        /*
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return base.AttackAction(selector, base.GunName);
            yield break;
        }*/
    }
}



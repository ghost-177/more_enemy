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
    public sealed class SampleCharacterTokenDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.GunName = GunNameID.GetGunFromId(400);
            //If IsPooled is false then the card cannot be discovered or added to the library at the end of combat.
            config.IsPooled = false;

            config.Colors = new List<ManaColor>() { ManaColor.Colorless };
            config.Cost = new ManaGroup() { Any = 2 };
            config.UpgradedCost = new ManaGroup() { Any = 1 };
            config.Rarity = Rarity.Common;

            config.Type = CardType.Attack;
            config.TargetType = TargetType.SingleEnemy;

            config.Damage = 15;
            config.UpgradedDamage = 20;

            config.Value1 = 1;

            config.Keywords = Keyword.Exile | Keyword.Ethereal;
            //Setting Upgrading Keyword only provides the keyword when the card is upgraded.    
            config.UpgradedKeywords = Keyword.Exile | Keyword.Ethereal | Keyword.Accuracy;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterTokenDef))]
    public sealed class SampleCharacterToken : SampleCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            //Attack all enemies, selector is set to Battle.AllAliveEnemies.
            yield return base.AttackAction(selector, base.GunName);
            yield return new DrawManyCardAction(base.Value1);
            yield break;
        }
    }
}



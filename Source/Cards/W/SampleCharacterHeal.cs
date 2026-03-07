using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Units;
using LBoL.Core.Battle.BattleActions;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterHealDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.Colors = new List<ManaColor>() { ManaColor.White };
            config.Cost = new ManaGroup() { White = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.SingleEnemy;

            config.Value1 = 10;
            config.UpgradedValue1 = 15;

            config.Value2 = 5;
            config.UpgradedValue2 = 10;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterHealDef))]
    public sealed class SampleCharacterHeal : SampleCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            EnemyUnit selectedEnemy = selector.SelectedEnemy;
            //The player loses life:
            yield return HealAction(base.Value1);
            //This is equvalent to:
            //yield return new HealAction(Battle.Player, selectedEnemy, base.Value1);

            //Target enemy loses hp.
            yield return new HealAction(Battle.Player, selectedEnemy, base.Value2);
            yield break;
        }
    }
}



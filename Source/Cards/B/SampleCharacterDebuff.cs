using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterDebuffDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Black };
            config.Cost = new ManaGroup() { Any = 2, Black = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 1, Black = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.SingleEnemy;

            config.Value1 = 1;
            config.UpgradedValue1 = 2;

            config.Value2 = 2;
            config.UpgradedValue2 = 3;

            //Add Weak, Vulnerable, Firepower Down descrption when hovering over the card.
            config.RelativeEffects = new List<string>() { nameof(Weak), nameof(Vulnerable), nameof(FirepowerNegative) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(Weak), nameof(Vulnerable), nameof(FirepowerNegative) };

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterDebuffDef))]
    public sealed class SampleCharacterDebuff : SampleCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            EnemyUnit selectedEnemy = selector.SelectedEnemy;
            //Buff and debuff have either a level or a duration.
            //Duration: Effects that last a certain amount of turns then disappear.
            //Level: Effects that have a fixed duration but that can vary in intensity. 
            //Weak and Vulnerable have a duration, FirepowerNegative has a level.  
            //DebuffAction's 2nd field is the level, the 3rd one is the duration.
            yield return base.DebuffAction<Weak>(selectedEnemy, 0, base.Value1, 0, 0, true, 0.2f);
            yield return base.DebuffAction<Vulnerable>(selectedEnemy, 0, base.Value1, 0, 0, true, 0.2f);
            yield return base.DebuffAction<FirepowerNegative>(selectedEnemy, base.Value2, 0, 0, 0, true, 0.2f);
            yield break;
        }
    }
}



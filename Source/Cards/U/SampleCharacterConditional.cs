using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Units;
using LBoL.Core.Battle.BattleActions;
using LBoL.EntityLib.StatusEffects.Basic;
using System.Linq;
using LBoL.Core.Intentions;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterConditionalDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Any = 1, Blue = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Nobody;

            config.Value1 = 12;
            config.UpgradedValue1 = 15;

            config.Value2 = 6;
            config.UpgradedValue2 = 8;

            config.Illustrator = "";

            config.RelativeEffects = new List<string>() { nameof(Reflect), nameof(TempElectric) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(Reflect), nameof(TempElectric) };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterConditionalDef))]
    public sealed class SampleCharacterConditional : SampleCharacterCard
    {

        //Code taken from the base game (Heart Blossoms / SatoriAttack.cs)
        //If any enemy intends to attack, returns True.
        public override bool Triggered
		{
			get
			{
                IEnumerable<EnemyUnit> enemies = base.Battle.AllAliveEnemies;
                foreach (EnemyUnit enemy in enemies)
                {
                    //Check whether any enemy intends to attack.
                    bool intendsToAttack = enemy.Intentions.Any(delegate(Intention i)
                    {
                        if (!(i is AttackIntention))
                        {
                            //If the enemy intent is a spellcard, check whether it deals damage.
                            SpellCardIntention spellCardIntention = i as SpellCardIntention;
                            if (spellCardIntention == null || spellCardIntention.Damage == null)
                            {
                                return false;
                            }
                        }
                        return true;
                    });
                    if (intendsToAttack)
                    {
                        return true;
                    } 
                }
                return false;
			}
		}
        
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return new ApplyStatusEffectAction<Reflect>(Battle.Player, base.Value1, 0, 0, 0, 0.2f);
            if (base.PlayInTriggered)
            {  
                yield return new ApplyStatusEffectAction<TempElectric>(Battle.Player, base.Value2, 0, 0, 0, 0.2f);
                //Warning: TempElectric is different from Electric.
                //TempElectric only lasts for the rest of the turn. Electric lasts for the rest of combat.
            }
            yield break;
        }
    }
}



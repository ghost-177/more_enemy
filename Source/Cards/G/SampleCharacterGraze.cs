using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.StatusEffects;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterGrazeDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Green };    
            config.Cost = new ManaGroup() { Any = 1, Green = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 0 };
            config.Rarity = Rarity.Common;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Self;

            config.Value1 = 2;

            config.Illustrator = "";
            
            config.RelativeEffects = new List<string>() { nameof(Graze) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(Graze) };

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterGrazeDef))]
    public sealed class SampleCharacterGraze : SampleCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return base.BuffAction<Graze>(base.Value1, 0, 0, 0, 0.2f);
            yield break;
        }
    }
}



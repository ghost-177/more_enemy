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
    public sealed class SampleCharacterBasicAbilityDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Any = 1, Blue = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Ability;
            config.TargetType = TargetType.Nobody;

            config.Value1 = 1;
            config.UpgradedValue1 = 2;

            config.Value2 = 1;
            config.UpgradedValue2 = 2;

            config.RelativeEffects = new List<string>() { nameof(Firepower), nameof(Spirit) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(Firepower), nameof(Spirit) };

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterBasicAbilityDef))]
    public sealed class SampleCharacterBasicAbility : SampleCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<Firepower>(base.Value1, 0, 0, 0, 0.2f);
            yield return BuffAction<Spirit>(base.Value2, 0, 0, 0, 0.2f);
            //This is equivalent to:
            //yield return new ApplyStatusEffectAction<Firepower>(Battle.Player, base.Value1, 0, 0, 0, 0.2f);
            //yield return new ApplyStatusEffectAction<Spirit>(Battle.Player, base.Value2, 0, 0, 0, 0.2f);
            yield break;
        }
    }
}



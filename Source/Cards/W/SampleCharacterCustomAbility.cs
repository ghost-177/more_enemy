using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using LBoL.Core.Battle;
using LBoL.Core;
using SampleCharacterMod.StatusEffects;
using LBoL.Core.Battle.BattleActions;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterCustomAbilityDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.Colors = new List<ManaColor>() { ManaColor.White };
            config.Cost = new ManaGroup() { Any = 4, White = 1 };
            config.Rarity = Rarity.Rare;

            config.Type = CardType.Ability;
            config.TargetType = TargetType.Nobody;

            config.Value1 = 1;
            config.UpgradedValue1 = 2;

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterCustomAbilityDef))]
    public sealed class SampleCharacterCustomAbility : SampleCharacterCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return BuffAction<SampleCharacterTurnGainSpiritSe>(level: base.Value1, duration: 0, limit: 0, count: 0, 0.2f);
            //This is equivalent to:
            //yield return new ApplyStatusEffectAction<SampleCharacterTurnGainSpiritSe>(Battle.Player, level: base.Value1, duration: 0, count: 0, limit:  0, 0.2f);
            yield break;
        }
    }
}



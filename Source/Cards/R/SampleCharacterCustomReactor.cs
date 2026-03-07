using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using LBoL.Core.Battle;
using LBoL.Core;
using SampleCharacterMod.BattleActions;
using LBoL.Core.Battle.BattleActions;
using SampleCharacterMod.StatusEffects;
using SampleCharacterMod.Patches;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterCustomReactorDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();

            config.Colors = new List<ManaColor>() { ManaColor.Red };
            config.Cost = new ManaGroup() { Any = 1 };
            config.Rarity = Rarity.Uncommon;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Nobody;

            config.Value1 = 1;
            config.Value2 = 1;

            config.RelativeEffects = new List<string>() { nameof(SampleCharacterEnhanceSe) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(SampleCharacterEnhanceSe) };

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterCustomReactorDef))]
    public sealed class SampleCharacterCustomReactor : SampleCharacterCard
    {
        protected override void OnEnterBattle(BattleController battle)
		{
            base.ReactBattleEvent<BuffAttackEventArgs>(CustomGameEventManager.PostCustomEvent, new EventSequencedReactor<BuffAttackEventArgs>(this.OnBuffAtack)); 
		}

        private IEnumerable<BattleAction> OnBuffAtack(BuffAttackEventArgs args)
        {
            //Activate a visual effect to notify that the card was changed.
            base.NotifyActivating();
            this.DeltaValue1 += Value2;
            //Update the card's textbox.
            base.NotifyChanged();
            yield break;
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return new DrawManyCardAction(base.Value1);
            yield break;
        }
    }
}



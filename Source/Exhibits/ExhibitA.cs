using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits;
using LBoLEntitySideloader.Attributes;

namespace SampleCharacterMod.Exhibits
{
    public sealed class SampleCharacterExhibitADef : SampleCharacterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            ExhibitConfig exhibitConfig = this.GetDefaultExhibitConfig();

            exhibitConfig.Value1 = 1;
            exhibitConfig.Mana = new ManaGroup() { Philosophy = 1 };
            exhibitConfig.BaseManaColor = ManaColor.Philosophy;
            exhibitConfig.RelativeCards = new List<string>() { nameof(PManaCard) };

            return exhibitConfig;
        }
    }

    [EntityLogic(typeof(SampleCharacterExhibitADef))]
    public sealed class SampleCharacterExhibitA : ShiningExhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent<UnitEventArgs>(base.Battle.Player.TurnStarted, new EventSequencedReactor<UnitEventArgs>(this.OnPlayerTurnStarted));
        }

        private IEnumerable<BattleAction> OnPlayerTurnStarted(UnitEventArgs args)
        {
            if (base.Battle.Player.TurnCounter == 1)
            {
                base.NotifyActivating();
                yield return new AddCardsToHandAction(new Card[] { Library.CreateCard<PManaCard>() });
            }
            yield break;
        }
    }
}
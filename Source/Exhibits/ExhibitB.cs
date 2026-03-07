using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.EntityLib.Exhibits;
using LBoLEntitySideloader.Attributes;

namespace SampleCharacterMod.Exhibits
{
    public sealed class SampleCharacterExhibitBDef : SampleCharacterExhibitTemplate
    {   
        public override ExhibitConfig MakeConfig()
        {

            ExhibitConfig exhibitConfig = this.GetDefaultExhibitConfig();
            exhibitConfig.Value1 = 1;
            exhibitConfig.Value2 = 1;
            exhibitConfig.Mana = new ManaGroup() { Green = 1 };
            exhibitConfig.BaseManaColor = ManaColor.Green;

            exhibitConfig.HasCounter = true;
            exhibitConfig.InitialCounter = 0;

            exhibitConfig.RelativeEffects = new List<string>() { };
            
            return exhibitConfig;
        }
    }

    [EntityLogic(typeof(SampleCharacterExhibitBDef))]
    public sealed class SampleCharacterExhibitB : ShiningExhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent<UnitEventArgs>(base.Battle.Player.TurnEnded, new EventSequencedReactor<UnitEventArgs>(this.OnPlayerTurnEnded));
            base.ReactBattleEvent<GameEventArgs>(base.Battle.BattleEnded, new EventSequencedReactor<GameEventArgs>(this.OnBattleEnded));
        }

        private IEnumerable<BattleAction> OnPlayerTurnEnded(UnitEventArgs args)
        {
            this.Counter += 1;
            yield break;
        }

        private IEnumerable<BattleAction> OnBattleEnded(GameEventArgs args)
        {
            this.Counter = 0;
            yield break;
        }
    }
}
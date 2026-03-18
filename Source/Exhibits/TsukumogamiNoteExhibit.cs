using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Exhibits;


namespace EternalWinterMod.Exhibits
{
    // 付喪神音符 — 战斗开始时获得 2 层火力
    public sealed class TsukumogamiNoteExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Rare;
            c.Value1 = 2;
            return c;
        }
    }

    [EntityLogic(typeof(TsukumogamiNoteExhibitDef))]
    public sealed class TsukumogamiNoteExhibit : Exhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent(base.Battle.BattleStarted,
                new EventSequencedReactor<GameEventArgs>(this.OnBattleStarted));
        }

        private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
        {
            base.NotifyActivating();
            yield return new ApplyStatusEffectAction(typeof(Firepower), base.Battle.Player, Config.Value1.Value, null, null, null, 0f, false);
        }
    }
}

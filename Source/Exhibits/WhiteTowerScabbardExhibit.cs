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
    // 白楼剑鞘碎片 — 战斗开始时获得 2 层闪避
    public sealed class WhiteTowerScabbardExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Uncommon;
            c.Value1 = 2;
            return c;
        }
    }

    [EntityLogic(typeof(WhiteTowerScabbardExhibitDef))]
    public sealed class WhiteTowerScabbardExhibit : Exhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent(base.Battle.BattleStarted,
                new EventSequencedReactor<GameEventArgs>(this.OnBattleStarted));
        }

        private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
        {
            base.NotifyActivating();
            yield return new ApplyStatusEffectAction(typeof(Graze), base.Battle.Player, Config.Value1.Value, null, null, null, 0f, false);
        }
    }
}

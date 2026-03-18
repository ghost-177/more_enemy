using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Exhibits;


namespace EternalWinterMod.Exhibits
{
    // 妖精冰晶 — 战斗开始时获得 8 点格挡
    public sealed class FairyIceCrystalExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Uncommon;
            c.Value1 = 8;
            return c;
        }
    }

    [EntityLogic(typeof(FairyIceCrystalExhibitDef))]
    public sealed class FairyIceCrystalExhibit : Exhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent(base.Battle.BattleStarted,
                new EventSequencedReactor<GameEventArgs>(this.OnBattleStarted));
        }

        private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
        {
            base.NotifyActivating();
            yield return new CastBlockShieldAction(base.Battle.Player, Config.Value1.Value, 0, BlockShieldType.Normal, false);
        }
    }
}

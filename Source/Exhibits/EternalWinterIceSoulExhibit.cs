using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Exhibits;
using LBoL.EntityLib.Exhibits;


namespace EternalWinterMod.Exhibits
{
    // 永冬冰魄石 — +1白法力，+15最大HP，战斗开始获得10点格挡
    public sealed class EternalWinterIceSoulExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Shining;
            c.Mana = new ManaGroup() { White = 1 };
            c.Value1 = 15; // HP
            c.Value2 = 10; // 格挡
            return c;
        }
    }

    [EntityLogic(typeof(EternalWinterIceSoulExhibitDef))]
    public sealed class EternalWinterIceSoulExhibit : ShiningExhibit
    {
        protected override void OnAdded(PlayerUnit player)
        {
            this.GameRun.GainMaxHp(Config.Value1.Value, false, false);
        }

        protected override void OnRemoved(PlayerUnit player)
        {
            this.GameRun.LoseMaxHp(Config.Value1.Value, false);
        }

        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent(base.Battle.BattleStarted,
                new EventSequencedReactor<GameEventArgs>(this.OnBattleStarted));
        }

        private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
        {
            base.NotifyActivating();
            yield return new CastBlockShieldAction(base.Battle.Player, Config.Value2.Value, 0, BlockShieldType.Normal, false);
        }
    }
}

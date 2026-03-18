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
    // 守矢神符 — +1绿法力，+12最大HP，每回合开始时若手牌数≤3获得5点格挡
    public sealed class MoriyaAmuletExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Shining;
            c.Mana = new ManaGroup() { Green = 1 };
            c.Value1 = 12; // HP
            c.Value2 = 3;  // 手牌阈值
            c.Value3 = 5;  // 格挡量
            return c;
        }
    }

    [EntityLogic(typeof(MoriyaAmuletExhibitDef))]
    public sealed class MoriyaAmuletExhibit : ShiningExhibit
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
            base.ReactBattleEvent(base.Battle.Player.TurnStarted,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnStarted));
        }

        private IEnumerable<BattleAction> OnTurnStarted(UnitEventArgs args)
        {
            if (base.Battle.HandZone.Count <= Config.Value2.Value)
            {
                base.NotifyActivating();
                yield return new CastBlockShieldAction(base.Battle.Player, Config.Value3.Value, 0, BlockShieldType.Normal, false);
            }
        }
    }
}

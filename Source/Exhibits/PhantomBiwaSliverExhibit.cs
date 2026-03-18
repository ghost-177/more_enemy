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
    // 幻奏琵琶片 — 回合内打出 3+ 张牌时，回合结束回复 3 点 HP
    public sealed class PhantomBiwaSliverExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Uncommon;
            c.Value1 = 3; // 触发打牌数
            c.Value2 = 3; // 回复量
            c.HasCounter = true;
            c.InitialCounter = 0;
            return c;
        }
    }

    [EntityLogic(typeof(PhantomBiwaSliverExhibitDef))]
    public sealed class PhantomBiwaSliverExhibit : Exhibit
    {
        protected override void OnEnterBattle()
        {
            this.Counter = 0;
            base.ReactBattleEvent(base.Battle.Player.TurnStarted,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnStarted));
            base.ReactBattleEvent(base.Battle.CardUsed,
                new EventSequencedReactor<CardUsingEventArgs>(this.OnCardUsed));
            base.ReactBattleEvent(base.Battle.Player.TurnEnded,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnEnded));
        }

        private IEnumerable<BattleAction> OnTurnStarted(UnitEventArgs args)
        {
            this.Counter = 0;
            yield break;
        }

        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            this.Counter++;
            yield break;
        }

        private IEnumerable<BattleAction> OnTurnEnded(UnitEventArgs args)
        {
            if (this.Counter >= Config.Value1.Value)
            {
                base.NotifyActivating();
                yield return new HealAction(base.Battle.Player, base.Battle.Player, Config.Value2.Value, HealType.Normal, 0f);
            }
        }
    }
}

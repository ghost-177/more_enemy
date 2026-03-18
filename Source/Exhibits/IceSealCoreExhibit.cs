using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Exhibits;


namespace EternalWinterMod.Exhibits
{
    // 寒冰封印核 — 手牌中有厄运牌时每回合开始额外获得 3 点格挡
    public sealed class IceSealCoreExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Rare;
            c.Value1 = 3;
            return c;
        }
    }

    [EntityLogic(typeof(IceSealCoreExhibitDef))]
    public sealed class IceSealCoreExhibit : Exhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent(base.Battle.Player.TurnStarted,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnStarted));
        }

        private IEnumerable<BattleAction> OnTurnStarted(UnitEventArgs args)
        {
            bool hasCurse = false;
            foreach (var card in base.Battle.HandZone)
            {
                if (card.Config.Type == CardType.Misfortune)
                {
                    hasCurse = true;
                    break;
                }
            }
            if (hasCurse)
            {
                base.NotifyActivating();
                yield return new CastBlockShieldAction(base.Battle.Player, Config.Value1.Value, 0, BlockShieldType.Normal, false);
            }
        }
    }
}

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
    // 幽幻剑气 — 本回合打出的第 3 张及以上攻击牌使用后，追加对主要目标 4 点穿透伤害
    public sealed class PhantomSwordAuraExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Rare;
            c.Value1 = 4; // 追加伤害
            return c;
        }
    }

    [EntityLogic(typeof(PhantomSwordAuraExhibitDef))]
    public sealed class PhantomSwordAuraExhibit : Exhibit
    {
        private int _attackCardsThisTurn;

        protected override void OnEnterBattle()
        {
            _attackCardsThisTurn = 0;
            base.ReactBattleEvent(base.Battle.Player.TurnStarted,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnStarted));
            base.ReactBattleEvent(base.Battle.CardUsed,
                new EventSequencedReactor<CardUsingEventArgs>(this.OnCardUsed));
        }

        private IEnumerable<BattleAction> OnTurnStarted(UnitEventArgs args)
        {
            _attackCardsThisTurn = 0;
            yield break;
        }

        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            if (args.Card.Config.Type != CardType.Attack)
                yield break;

            _attackCardsThisTurn++;
            if (_attackCardsThisTurn >= 3)
            {
                var targets = base.Battle.AllAliveEnemies;
                foreach (var enemy in targets)
                {
                    base.NotifyActivating();
                    yield return new DamageAction(base.Battle.Player, enemy,
                        DamageInfo.HpLose(Config.Value1.Value, false), "", GunType.Single);
                    break; // 只打第一个目标（主目标）
                }
            }
        }
    }
}

using System.Collections.Generic;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Cards;


namespace EternalWinterMod.Enemies.Act2
{
    [EntityLogic(typeof(KappaMechanicEWDef))]
    public sealed class KappaMechanicEW : EnemyUnit
    {
        // AI 节奏：普通弹射 → 动力炮击（重击+将冰霜水晶加入玩家弃牌堆）→ 循环（2回合）
        private int _turnCounter = 0;

        public string GadgetShotMoveName => base.GetSpellCardName(new int?(0), 0);
        public string PowerCannonMoveName => base.GetSpellCardName(new int?(0), 1);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
            ReactBattleEvent(Battle.BattleStarted, OnBattleStarted);
        }

        private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
        {
            // 显示被动SE标识
            yield return new ApplyStatusEffectAction(
                typeof(KappaMechanicInventionSE), this, null, null, null, null, 0f, false);
            // 战斗开始向玩家弃牌堆加入2张「故障机关」厄运牌
            var cards = new System.Collections.Generic.List<LBoL.Core.Cards.Card>
            {
                Library.CreateCard(typeof(BrokenDevice)),
                Library.CreateCard(typeof(BrokenDevice)),
            };
            yield return new AddCardsToDiscardAction(cards, AddCardsType.Normal);
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            if (_turnCounter == 0)
            {
                yield return base.AttackMove(this.GadgetShotMoveName, base.Gun1, base.Damage1);
            }
            else
            {
                // 动力炮击：重击 + 将冰霜水晶加入玩家弃牌堆
                yield return base.AttackMove(this.PowerCannonMoveName, base.Gun2, base.Damage2);
                yield return base.AddCardMove(this.PowerCannonMoveName, typeof(FrostCrystal), 1, EnemyUnit.AddCardZone.Discard, null, false);
            }
        }

        protected override void UpdateMoveCounters()
        {
            _turnCounter = (_turnCounter + 1) % 2;
        }
    }
}

using System.Collections.Generic;
using LBoL.Core.Battle;
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

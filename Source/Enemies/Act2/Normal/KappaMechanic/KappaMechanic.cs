using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act2
{
    [EntityLogic(typeof(KappaMechanicEWDef))]
    public sealed class KappaMechanicEW : EnemyUnit
    {
        // AI 节奏：普通弹射 → 蓄力炮击（2回合周期，纯输出）
        private int _turnCounter = 0;

        public string GadgetShotMoveName => base.GetSpellCardName(new int?(0), 0);
        public string PowerShotMoveName  => base.GetSpellCardName(new int?(0), 1);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            if (_turnCounter == 0)
                yield return base.AttackMove(this.GadgetShotMoveName, base.Gun1, base.Damage1);
            else
                yield return base.AttackMove(this.PowerShotMoveName, base.Gun2, base.Damage2);
        }

        protected override void UpdateMoveCounters()
        {
            _turnCounter = (_turnCounter + 1) % 2;
        }
    }
}

using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act3
{
    [EntityLogic(typeof(HiganbanaWraithEWDef))]
    public sealed class HiganbanaWraithEW : EnemyUnit
    {
        // AI 节奏：攻击 → 防御 → 攻击 → 攻击（4回合周期）
        private int _turnCounter = 0;

        public string AttackMoveName => base.GetSpellCardName(new int?(0), 0);
        public string DefendMoveName => base.GetSpellCardName(new int?(0), 1);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            if (_turnCounter == 1)
                yield return base.DefendMove(this, this.DefendMoveName, base.Defend, 0, 0, true, null);
            else
                yield return base.AttackMove(this.AttackMoveName, base.Gun1, base.Damage1);
        }

        protected override void UpdateMoveCounters()
        {
            _turnCounter = (_turnCounter + 1) % 4;
        }
    }
}

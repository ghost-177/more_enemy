using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act2
{
    [EntityLogic(typeof(MountainFairyDef))]
    public sealed class MountainFairy : EnemyUnit
    {
        // AI 节奏：防御 → 攻击 → 攻击（3回合周期，先手防御）
        private int _turnCounter = 0;

        public string AttackMoveName => base.GetSpellCardName(new int?(0), 0);
        public string DefendMoveName => base.GetSpellCardName(new int?(0), 1);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            if (_turnCounter == 0)
                yield return base.DefendMove(this, this.DefendMoveName, base.Defend, 0, 0, true, null);
            else
                yield return base.AttackMove(this.AttackMoveName, base.Gun1, base.Damage1);
        }

        protected override void UpdateMoveCounters()
        {
            _turnCounter = (_turnCounter + 1) % 3;
        }
    }
}

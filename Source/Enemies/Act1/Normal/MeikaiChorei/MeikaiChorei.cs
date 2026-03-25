using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act1
{
    [EntityLogic(typeof(MeikaiChoreiEWDef))]
    public sealed class MeikaiChoreiEW : EnemyUnit
    {
        // AI 节奏：攻击 → 攻击 → 幽灵薄纱（施弱）→ 循环（3回合周期）
        private int _turnCounter = 0;

        public string AttackMoveName  => base.GetSpellCardName(new int?(0), 0);
        public string GhostlyVeilName => base.GetSpellCardName(new int?(0), 1);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            if (_turnCounter == 2)
            {
                // 幽灵薄纱：对玩家施加虚弱(1)
                yield return base.NegativeMove(this.GhostlyVeilName, typeof(Weak), 1, null, true, false, null);
            }
            else
            {
                yield return base.AttackMove(this.AttackMoveName, base.Gun1, base.Damage1);
            }
        }

        protected override void UpdateMoveCounters()
        {
            _turnCounter = (_turnCounter + 1) % 3;
        }
    }
}

using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act2
{
    [EntityLogic(typeof(TenguScoutEWDef))]
    public sealed class TenguScoutEW : EnemyUnit
    {
        // AI 节奏：风刃斩（攻击）→ 戒备姿态（自身+火力1）→ 循环（交替）
        private bool _lastWasAttack = false;

        public string WindSlashMoveName   => base.GetSpellCardName(new int?(0), 0);
        public string GuardStanceMoveName => base.GetSpellCardName(new int?(0), 1);

        protected override void OnEnterBattle(BattleController battle)
        {
            _lastWasAttack = false;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            if (!_lastWasAttack)
            {
                yield return base.AttackMove(this.WindSlashMoveName, base.Gun1, base.Damage1);
            }
            else
            {
                // 戒备姿态：自身获得火力1
                yield return base.PositiveMove(this.GuardStanceMoveName, typeof(Firepower), 1, null, false, null);
            }
        }

        protected override void UpdateMoveCounters()
        {
            _lastWasAttack = !_lastWasAttack;
        }
    }
}

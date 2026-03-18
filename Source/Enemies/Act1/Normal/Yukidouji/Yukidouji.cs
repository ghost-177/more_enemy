using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act1
{
    [EntityLogic(typeof(YukidoujiDef))]
    public sealed class Yukidouji : EnemyUnit
    {
        private enum MoveType
        {
            Attack,
            Defend,
        }

        private MoveType Last { get; set; }
        private MoveType Next { get; set; }

        public string AttackMoveName => base.GetSpellCardName(new int?(0), 0);
        public string DefendMoveName => base.GetSpellCardName(new int?(0), 1);

        protected override void OnEnterBattle(BattleController battle)
        {
            this.Last = MoveType.Defend;
            this.Next = MoveType.Attack;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            switch (this.Next)
            {
                case MoveType.Attack:
                    yield return base.AttackMove(this.AttackMoveName, base.Gun1, base.Damage1);
                    this.Last = MoveType.Attack;
                    yield break;
                case MoveType.Defend:
                    yield return base.DefendMove(this, this.DefendMoveName, base.Defend, 0, 0, true, null);
                    this.Last = MoveType.Defend;
                    yield break;
            }
            yield break;
        }

        protected override void UpdateMoveCounters()
        {
            this.Next = (this.Last == MoveType.Attack) ? MoveType.Defend : MoveType.Attack;
        }
    }
}

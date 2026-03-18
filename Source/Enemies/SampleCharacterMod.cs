using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies
{
    [EntityLogic(typeof(SampleCharacterEnemyUnitDef))]
    public sealed class EternalWinterMod : EnemyUnit
    {
        //Internal list of the boss moves
        private enum MoveType
		{
			BasicAttack,
			BasicDefend,
		}

        //Internal parameters use to track the last move used by the boss.
        private MoveType Last { get; set; }

		private MoveType Next { get; set; }

        //Get the moves names
        public string BasicAttackMove 
        { 
            get
            { 
                return base.GetSpellCardName(new int?(0), 0);
            }
        }

        public string BasicDefendMove 
        { 
            get
            { 
                return base.GetSpellCardName(new int?(0), 1);
            }
        }

        protected override void OnEnterBattle(BattleController battle)
		{
            this.Last = MoveType.BasicDefend;
			this.Next = MoveType.BasicAttack;
		}

        //Action for the turn.
        protected override IEnumerable<IEnemyMove> GetTurnMoves()
		{
			switch (this.Next)
			{
                case MoveType.BasicAttack:
			    {
                    yield return base.AttackMove(this.BasicAttackMove, base.Gun1, base.Damage1);
                    this.Last = MoveType.BasicAttack;
                    yield break;
                }
                case MoveType.BasicDefend:
			    {
                    yield return new SimpleEnemyMove(Intention.Defend().WithMoveName(base.GetMove(1)), this.BasicDefendAction());
                    this.Last = MoveType.BasicDefend;
                    yield break;
                }
            }
            yield break;
        }

        //Perform a custom action
        private IEnumerable<BattleAction> BasicDefendAction()
        {
            yield return new EnemyMoveAction(this, base.GetMove(1));
            yield return new CastBlockShieldAction(this, base.Defend, 0);
        }

        //Update choose the next attack.
        protected override void UpdateMoveCounters()
		{
            this.Next = (this.Last == MoveType.BasicAttack) ? MoveType.BasicDefend : MoveType.BasicAttack ;
        }
    }
}
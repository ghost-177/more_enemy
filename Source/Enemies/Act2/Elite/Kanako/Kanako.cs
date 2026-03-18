using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act2
{
    [EntityLogic(typeof(KanakoEWDef))]
    public sealed class Kanako : EnemyUnit
    {
        // AI 节奏：神风 → 御柱 → 防御（3回合周期）
        private int _turnCounter = 0;

        public string DivineWindMoveName => base.GetSpellCardName(new int?(0), 0);
        public string PillarMoveName     => base.GetSpellCardName(new int?(0), 1);
        public string DefendMoveName     => base.GetSpellCardName(new int?(0), 2);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            switch (_turnCounter)
            {
                case 0:
                    yield return base.AttackMove(this.DivineWindMoveName, base.Gun1, base.Damage1);
                    yield break;
                case 1:
                    yield return base.AttackMove(this.PillarMoveName, base.Gun2, base.Damage2);
                    yield break;
                case 2:
                    yield return base.DefendMove(this, this.DefendMoveName, base.Defend, 0, 0, true, null);
                    yield break;
            }
            yield break;
        }

        protected override void UpdateMoveCounters()
        {
            _turnCounter = (_turnCounter + 1) % 3;
        }
    }
}

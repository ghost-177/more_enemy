using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act2
{
    [EntityLogic(typeof(SuwakoEWDef))]
    public sealed class SuwakoEW : EnemyUnit
    {
        // AI 节奏（4回合周期）：
        //   0: 蛙击（普通）
        //   1: 大地震（重击）
        //   2: 防御
        //   3: 铁环×2（Count1）
        private int _turnCounter = 0;

        public string FrogStrikeMoveName => base.GetSpellCardName(new int?(0), 0);
        public string EarthquakeMoveName => base.GetSpellCardName(new int?(0), 1);
        public string DefendMoveName     => base.GetSpellCardName(new int?(0), 2);
        public string IronRingMoveName   => base.GetSpellCardName(new int?(0), 3);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            switch (_turnCounter)
            {
                case 0:
                    yield return base.AttackMove(this.FrogStrikeMoveName, base.Gun1, base.Damage1);
                    yield break;
                case 1:
                    yield return base.AttackMove(this.EarthquakeMoveName, base.Gun2, base.Damage2);
                    yield break;
                case 2:
                    yield return base.DefendMove(this, this.DefendMoveName, base.Defend, 0, 0, true, null);
                    yield break;
                case 3:
                    // Count1 槽位存放铁环连击次数（=2）
                    yield return base.AttackMove(this.IronRingMoveName, base.Gun3, base.Damage3, base.Count1, false, null, false);
                    yield break;
            }
            yield break;
        }

        protected override void UpdateMoveCounters()
        {
            _turnCounter = (_turnCounter + 1) % 4;
        }
    }
}

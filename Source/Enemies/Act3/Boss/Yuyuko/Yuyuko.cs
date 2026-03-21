using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act3
{
    [EntityLogic(typeof(YuyukoEWDef))]
    public sealed class YuyukoEW : EnemyUnit
    {
        // AI 节奏：幽灵蝶舞 → 春死满开 → 防御 → 幽冥结界×Count1（4回合周期）
        private int _turnCounter = 0;

        public string GhostDanceMoveName  => base.GetSpellCardName(new int?(0), 0);
        public string SpringDeathMoveName => base.GetSpellCardName(new int?(0), 1);
        public string DefendMoveName      => base.GetSpellCardName(new int?(0), 2);
        public string NetherBarrierMoveName => base.GetSpellCardName(new int?(0), 3);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            switch (_turnCounter)
            {
                case 0:
                    yield return base.AttackMove(this.GhostDanceMoveName, base.Gun1, base.Damage1);
                    yield break;
                case 1:
                    yield return base.AttackMove(this.SpringDeathMoveName, base.Gun2, base.Damage2);
                    yield break;
                case 2:
                    yield return base.DefendMove(this, this.DefendMoveName, base.Defend, 0, 0, true, null);
                    yield break;
                case 3:
                    yield return base.AttackMove(this.NetherBarrierMoveName, base.Gun1, base.Damage3, base.Count1, false, null, true);
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

using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act3
{
    [EntityLogic(typeof(YoumuPhantomEWDef))]
    public sealed class YoumuPhantomEW : EnemyUnit
    {
        // AI 节奏：人刀击 → 幽灵半身斩×2 → 防御（3回合周期）
        private int _turnCounter = 0;

        public string SwordMoveName   => base.GetSpellCardName(new int?(0), 0);
        public string PhantomMoveName => base.GetSpellCardName(new int?(0), 1);
        public string DefendMoveName  => base.GetSpellCardName(new int?(0), 2);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            switch (_turnCounter)
            {
                case 0:
                    yield return base.AttackMove(this.SwordMoveName, base.Gun1, base.Damage1);
                    yield break;
                case 1:
                    yield return base.AttackMove(this.PhantomMoveName, base.Gun2, base.Damage2, base.Count2, false, null, false);
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

using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act1
{
    [EntityLogic(typeof(KaguyaEWDef))]
    public sealed class Kaguya : EnemyUnit
    {
        // AI 节奏：主攻击 → 连击（×2）→ 防御 → 循环（3回合周期）
        private int _turnCounter = 0;

        public string Attack1MoveName => base.GetSpellCardName(new int?(0), 0);
        public string Attack2MoveName => base.GetSpellCardName(new int?(0), 1);
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
                    yield return base.AttackMove(this.Attack1MoveName, base.Gun1, base.Damage1);
                    yield break;
                case 1:
                    // 2连击
                    yield return base.AttackMove(this.Attack2MoveName, base.Gun2, base.Damage2, base.Count2, false, null, false);
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

using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act1
{
    [EntityLogic(typeof(LettyEWDef))]
    public sealed class Letty : EnemyUnit
    {
        // AI 节奏（4回合周期）：
        //   0: 冰击（普通攻击）
        //   1: 暴雪（单次重击）
        //   2: 防御
        //   3: 寒霜弹（×2 连击）
        private int _turnCounter = 0;

        public string IceAttackMoveName  => base.GetSpellCardName(new int?(0), 0);
        public string BlizzardMoveName   => base.GetSpellCardName(new int?(0), 1);
        public string DefendMoveName     => base.GetSpellCardName(new int?(0), 2);
        public string FrostShotMoveName  => base.GetSpellCardName(new int?(0), 3);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            switch (_turnCounter)
            {
                case 0:
                    yield return base.AttackMove(this.IceAttackMoveName, base.Gun1, base.Damage1);
                    yield break;
                case 1:
                    yield return base.AttackMove(this.BlizzardMoveName, base.Gun2, base.Damage2);
                    yield break;
                case 2:
                    yield return base.DefendMove(this, this.DefendMoveName, base.Defend, 0, 0, true, null);
                    yield break;
                case 3:
                    // Count1 槽位存放寒霜弹连击次数（默认=2）
                    yield return base.AttackMove(this.FrostShotMoveName, base.Gun3, base.Damage3, base.Count1, false, null, false);
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

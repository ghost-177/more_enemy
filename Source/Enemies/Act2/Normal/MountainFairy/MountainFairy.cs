using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act2
{
    [EntityLogic(typeof(MountainFairyEWDef))]
    public sealed class MountainFairyEW : EnemyUnit
    {
        // AI 节奏：束缚咒（施弱2）→ 灵气护盾（防御）→ 攻击 → 循环（3回合周期）
        private int _turnCounter = 0;

        public string BindingCurseName  => base.GetSpellCardName(new int?(0), 0);
        public string SpiritBarrierName => base.GetSpellCardName(new int?(0), 1);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            switch (_turnCounter)
            {
                case 0:
                    // 束缚咒：对玩家施加虚弱(2)
                    yield return base.NegativeMove(this.BindingCurseName, typeof(Weak), 2, null, true, false, null);
                    yield break;
                case 1:
                    yield return base.DefendMove(this, this.SpiritBarrierName, base.Defend, 0, 0, true, null);
                    yield break;
                case 2:
                    yield return base.AttackMove(this.BindingCurseName, base.Gun1, base.Damage1);
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

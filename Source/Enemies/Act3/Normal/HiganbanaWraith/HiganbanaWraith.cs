using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act3
{
    [EntityLogic(typeof(HiganbanaWraithEWDef))]
    public sealed class HiganbanaWraithEW : EnemyUnit
    {
        // AI 节奏：夺魂（攻击+施弱1）→ 幽灵薄纱（防御）→ 夺魂 → 夺魂（4回合周期）
        private int _turnCounter = 0;

        public string SoulDrainMoveName  => base.GetSpellCardName(new int?(0), 0);
        public string SpiritVeilMoveName => base.GetSpellCardName(new int?(0), 1);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            if (_turnCounter == 1)
            {
                yield return base.DefendMove(this, this.SpiritVeilMoveName, base.Defend, 0, 0, true, null);
            }
            else
            {
                // 夺魂：攻击并对玩家施加虚弱(1)
                yield return base.AttackMove(this.SoulDrainMoveName, base.Gun1, base.Damage1);
                yield return base.NegativeMove(this.SoulDrainMoveName, typeof(Weak), null, 1, false, false, null);
            }
        }

        protected override void UpdateMoveCounters()
        {
            _turnCounter = (_turnCounter + 1) % 4;
        }
    }
}

using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Cards;


namespace EternalWinterMod.Enemies.Act3
{
    [EntityLogic(typeof(NetherMessengerEWDef))]
    public sealed class NetherMessengerEW : EnemyUnit
    {
        // AI 节奏：裁决一击（攻击）→ 冥界护盾（将冥界裁决加入玩家弃牌堆）→ 攻击 → 循环
        private int _turnCounter = 0;

        public string VerdictStrikeMoveName => base.GetSpellCardName(new int?(0), 0);
        public string NetherShieldMoveName  => base.GetSpellCardName(new int?(0), 1);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            if (_turnCounter == 1)
            {
                // 冥界护盾：将1张冥界裁决置入玩家弃牌堆
                yield return base.AddCardMove(this.NetherShieldMoveName, typeof(NetherworldVerdict), 1, EnemyUnit.AddCardZone.Discard, null, false);
            }
            else
            {
                yield return base.AttackMove(this.VerdictStrikeMoveName, base.Gun1, base.Damage1);
            }
        }

        protected override void UpdateMoveCounters()
        {
            _turnCounter = (_turnCounter + 1) % 3;
        }
    }
}

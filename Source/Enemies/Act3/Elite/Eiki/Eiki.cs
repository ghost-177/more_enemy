using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Cards;


namespace EternalWinterMod.Enemies.Act3
{
    [EntityLogic(typeof(EikiEWDef))]
    public sealed class EikiEW : EnemyUnit
    {
        // AI 节奏：审判（施易伤2）→ 裁决（将冥界法令加入玩家弃牌堆）→ 审判 → 公正防壁（防御）（4回合周期）
        private int _turnCounter = 0;

        public string JudgmentMoveName     => base.GetSpellCardName(new int?(0), 0);
        public string VerdictMoveName      => base.GetSpellCardName(new int?(0), 1);
        public string RighteousBarrierName => base.GetSpellCardName(new int?(0), 2);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            switch (_turnCounter)
            {
                case 0:
                case 2:
                    // 审判：对玩家施加易伤(2)
                    yield return base.NegativeMove(this.JudgmentMoveName, typeof(Vulnerable), null, 2, false, false, null);
                    yield break;
                case 1:
                    // 裁决：将1张冥界法令置入玩家弃牌堆
                    yield return base.AddCardMove(this.VerdictMoveName, typeof(NetherworldDecree), 1, EnemyUnit.AddCardZone.Discard, null, false);
                    yield break;
                case 3:
                    yield return base.DefendMove(this, this.RighteousBarrierName, base.Defend, 0, 0, true, null);
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

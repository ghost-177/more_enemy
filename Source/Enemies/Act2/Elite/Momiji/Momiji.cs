using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act2
{
    [EntityLogic(typeof(MomijiEWDef))]
    public sealed class MomijiEW : EnemyUnit
    {
        // AI 节奏：刀击 → 速斩×2 → 刀击 → 防御（4回合周期）
        private int _turnCounter = 0;

        public string SlashMoveName  => base.GetSpellCardName(new int?(0), 0);
        public string RapidMoveName  => base.GetSpellCardName(new int?(0), 1);
        public string DefendMoveName => base.GetSpellCardName(new int?(0), 2);

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
                    yield return base.AttackMove(this.SlashMoveName, base.Gun1, base.Damage1);
                    yield break;
                case 1:
                    yield return base.AttackMove(this.RapidMoveName, base.Gun2, base.Damage2, base.Count2, false, null, false);
                    yield break;
                case 3:
                    // 盾守：自身获得灵力(2)，使防御更有效
                    yield return base.PositiveMove(this.DefendMoveName, typeof(Spirit), 2, null, false, null);
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

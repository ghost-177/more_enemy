using System.Collections.Generic;
using LBoL.Core.Battle;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act1
{
    [EntityLogic(typeof(MaiyoTsukiEWDef))]
    public sealed class MaiyoTsukiEW : EnemyUnit
    {
        public string AttackMoveName => base.GetSpellCardName(new int?(0), 0);

        protected override void OnEnterBattle(BattleController battle) { }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            // 迷途月兔只攻击，不防御
            yield return base.AttackMove(this.AttackMoveName, base.Gun1, base.Damage1);
        }

        protected override void UpdateMoveCounters() { }
    }
}

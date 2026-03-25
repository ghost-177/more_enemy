using System.Collections.Generic;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;


namespace EternalWinterMod.Enemies.Act1
{
    [EntityLogic(typeof(LettyEWDef))]
    public sealed class LettyEW : EnemyUnit
    {
        // AI 节奏（4回合周期）：
        //   0: 冰弹连射（普通攻击）
        //   1: 极寒封冻（施弱）
        //   2: 寒冬薄纱（防御）
        //   3: 冰晶淋浴（×Count1 连击）
        // 被动：冻结领域（LettyFreezeAuraSE）— 每回合锁定玩家 Power 点法力
        private int _turnCounter = 0;

        public string IceBarrageMoveName  => base.GetSpellCardName(new int?(0), 0);
        public string PerfectFreezeMoveName => base.GetSpellCardName(new int?(0), 1);
        public string WinterVeilMoveName  => base.GetSpellCardName(new int?(0), 2);
        public string FrostShowerMoveName => base.GetSpellCardName(new int?(0), 3);

        protected override void OnEnterBattle(BattleController battle)
        {
            _turnCounter = 0;
            // 监听战斗开始事件，在事件回调中施加SE（不能在void方法里直接yield BattleAction）
            ReactBattleEvent(Battle.BattleStarted, OnBattleStarted);
        }

        private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
        {
            // 施加永久被动「冻结领域」到蕾蒂自身，Level = Power（决定每回合锁定多少法力）
            yield return new ApplyStatusEffectAction(
                typeof(LettyFreezeAuraSE),
                this,
                Power,          // level = Power
                null,           // duration = null（永久）
                null,           // count
                null,           // limit
                0f,             // occupationTime
                false           // startAutoDecreasing
            );
        }

        protected override IEnumerable<IEnemyMove> GetTurnMoves()
        {
            switch (_turnCounter)
            {
                case 0:
                    yield return base.AttackMove(this.IceBarrageMoveName, base.Gun1, base.Damage1);
                    yield break;
                case 1:
                    // 极寒封冻：对玩家施加虚弱(2)
                    yield return base.NegativeMove(this.PerfectFreezeMoveName, typeof(Weak), null, 2, false, false, null);
                    yield break;
                case 2:
                    yield return base.DefendMove(this, this.WinterVeilMoveName, base.Defend, 0, 0, true, null);
                    yield break;
                case 3:
                    yield return base.AttackMove(this.FrostShowerMoveName, base.Gun3, base.Damage3, base.Count1, false, null, false);
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

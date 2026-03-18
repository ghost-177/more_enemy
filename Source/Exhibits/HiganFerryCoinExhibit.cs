using System.Collections.Generic;
using System.Linq;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Exhibits;


namespace EternalWinterMod.Exhibits
{
    // 三途渡费铜钱 — 每场战斗胜利后，回复最大 HP 的 8%
    public sealed class HiganFerryCoinExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Rare;
            c.Value1 = 8; // 百分比
            return c;
        }
    }

    [EntityLogic(typeof(HiganFerryCoinExhibitDef))]
    public sealed class HiganFerryCoinExhibit : Exhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent(base.Battle.BattleEnded,
                new EventSequencedReactor<GameEventArgs>(this.OnBattleEnded));
        }

        private IEnumerable<BattleAction> OnBattleEnded(GameEventArgs args)
        {
            // 仅在全部敌人死亡时（胜利）触发
            if (base.Battle.AllAliveEnemies.Any())
                yield break;

            base.NotifyActivating();
            int healAmount = base.Battle.Player.MaxHp * Config.Value1.Value / 100;
            if (healAmount > 0)
                yield return new HealAction(base.Battle.Player, base.Battle.Player, healAmount, HealType.Normal, 0f);
        }
    }
}

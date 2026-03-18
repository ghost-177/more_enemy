using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Exhibits;
using LBoL.EntityLib.Exhibits;


namespace EternalWinterMod.Exhibits
{
    // 反魂蝶翅膀 — +1无色法力，攻击+1，击杀敌人时回复8点HP
    public sealed class ReincarnationButterflyWingExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Shining;
            c.Mana = new ManaGroup() { Colorless = 1 };
            c.Value1 = 1;  // 火力
            c.Value2 = 8;  // 回复量
            return c;
        }
    }

    [EntityLogic(typeof(ReincarnationButterflyWingExhibitDef))]
    public sealed class ReincarnationButterflyWingExhibit : ShiningExhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent(base.Battle.BattleStarted,
                new EventSequencedReactor<GameEventArgs>(this.OnBattleStarted));
            base.ReactBattleEvent(base.Battle.EnemyDied,
                new EventSequencedReactor<DieEventArgs>(this.OnEnemyDied));
        }

        private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
        {
            base.NotifyActivating();
            yield return new ApplyStatusEffectAction(typeof(Firepower), base.Battle.Player, Config.Value1.Value, null, null, null, 0f, false);
        }

        private IEnumerable<BattleAction> OnEnemyDied(DieEventArgs args)
        {
            base.NotifyActivating();
            yield return new HealAction(base.Battle.Player, base.Battle.Player, Config.Value2.Value, HealType.Normal, 0f);
        }
    }
}

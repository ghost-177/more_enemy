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


namespace EternalWinterMod.Exhibits
{
    // 死神的大镰残片 — 战斗开始获得 1 层火力；每击杀一个敌人额外获得 1 层（上限 3 层合计）
    public sealed class DeathScytheFragmentExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Uncommon;
            c.Value1 = 3; // 总上限
            c.HasCounter = true;
            c.InitialCounter = 0;
            return c;
        }
    }

    [EntityLogic(typeof(DeathScytheFragmentExhibitDef))]
    public sealed class DeathScytheFragmentExhibit : Exhibit
    {
        protected override void OnEnterBattle()
        {
            this.Counter = 0;
            base.ReactBattleEvent(base.Battle.BattleStarted,
                new EventSequencedReactor<GameEventArgs>(this.OnBattleStarted));
            base.ReactBattleEvent(base.Battle.EnemyDied,
                new EventSequencedReactor<DieEventArgs>(this.OnEnemyDied));
        }

        private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
        {
            this.Counter = 1;
            base.NotifyActivating();
            yield return new ApplyStatusEffectAction(typeof(Firepower), base.Battle.Player, 1, null, null, null, 0f, false);
        }

        private IEnumerable<BattleAction> OnEnemyDied(DieEventArgs args)
        {
            if (this.Counter < Config.Value1.Value)
            {
                this.Counter++;
                base.NotifyActivating();
                yield return new ApplyStatusEffectAction(typeof(Firepower), base.Battle.Player, 1, null, null, null, 0f, false);
            }
        }
    }
}

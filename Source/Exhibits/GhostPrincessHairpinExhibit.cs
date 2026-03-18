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
    // 幽灵公主蝴蝶发簪 — +1黑法力，+20最大HP，战斗开始获得2层火力
    public sealed class GhostPrincessHairpinExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Shining;
            c.Mana = new ManaGroup() { Black = 1 };
            c.Value1 = 20; // HP
            c.Value2 = 2;  // 火力
            return c;
        }
    }

    [EntityLogic(typeof(GhostPrincessHairpinExhibitDef))]
    public sealed class GhostPrincessHairpinExhibit : ShiningExhibit
    {
        protected override void OnAdded(PlayerUnit player)
        {
            this.GameRun.GainMaxHp(Config.Value1.Value, false, false);
        }

        protected override void OnRemoved(PlayerUnit player)
        {
            this.GameRun.LoseMaxHp(Config.Value1.Value, false);
        }

        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent(base.Battle.BattleStarted,
                new EventSequencedReactor<GameEventArgs>(this.OnBattleStarted));
        }

        private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
        {
            base.NotifyActivating();
            yield return new ApplyStatusEffectAction(typeof(Firepower), base.Battle.Player, Config.Value2.Value, null, null, null, 0f, false);
        }
    }
}

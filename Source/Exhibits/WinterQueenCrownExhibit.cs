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
    // 寒冬女王冠冕 — +1白法力，攻击基础值+1，战斗开始额外多抽1张牌
    public sealed class WinterQueenCrownExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Shining;
            c.Mana = new ManaGroup() { White = 1 };
            c.Value1 = 1; // 火力层数
            c.Value2 = 1; // 抽牌数
            return c;
        }
    }

    [EntityLogic(typeof(WinterQueenCrownExhibitDef))]
    public sealed class WinterQueenCrownExhibit : ShiningExhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent(base.Battle.BattleStarted,
                new EventSequencedReactor<GameEventArgs>(this.OnBattleStarted));
        }

        private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
        {
            base.NotifyActivating();
            yield return new ApplyStatusEffectAction(typeof(Firepower), base.Battle.Player, Config.Value1.Value, null, null, null, 0f, false);
            yield return new DrawManyCardAction(Config.Value2.Value);
        }
    }
}

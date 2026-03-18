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
    // 御柱碎片 — +1无色法力，攻击+2（战斗开始），每打出5张牌额外+1永久火力（上限3层）
    public sealed class OnbashiraFragmentExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Shining;
            c.Mana = new ManaGroup() { Colorless = 1 };
            c.Value1 = 2; // 初始火力层数
            c.Value2 = 5; // 触发出牌数
            c.Value3 = 3; // 额外火力上限
            c.HasCounter = true;
            c.InitialCounter = 0;
            return c;
        }
    }

    [EntityLogic(typeof(OnbashiraFragmentExhibitDef))]
    public sealed class OnbashiraFragmentExhibit : Exhibit
    {
        private int _cardsPlayedTotal;
        private int _bonusFirepowerGained;

        protected override void OnEnterBattle()
        {
            _cardsPlayedTotal = 0;
            _bonusFirepowerGained = 0;
            base.ReactBattleEvent(base.Battle.BattleStarted,
                new EventSequencedReactor<GameEventArgs>(this.OnBattleStarted));
            base.ReactBattleEvent(base.Battle.CardUsed,
                new EventSequencedReactor<CardUsingEventArgs>(this.OnCardUsed));
        }

        private IEnumerable<BattleAction> OnBattleStarted(GameEventArgs args)
        {
            base.NotifyActivating();
            yield return new ApplyStatusEffectAction(typeof(Firepower), base.Battle.Player, Config.Value1.Value, null, null, null, 0f, false);
        }

        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            _cardsPlayedTotal++;
            if (_cardsPlayedTotal % Config.Value2.Value == 0 && _bonusFirepowerGained < Config.Value3.Value)
            {
                _bonusFirepowerGained++;
                this.Counter = _bonusFirepowerGained;
                base.NotifyActivating();
                yield return new ApplyStatusEffectAction(typeof(Firepower), base.Battle.Player, 1, null, null, null, 0f, false);
            }
        }
    }
}

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
    // 式神尾铃 — 回合内受到 10+ 点伤害时获得 1 层火力（本场最多 3 次）
    public sealed class ShikigamiBellExhibitDef : EternalWinterExhibitTemplate
    {
        public override ExhibitConfig MakeConfig()
        {
            var c = GetDefaultExhibitConfig();
            c.Rarity = Rarity.Uncommon;
            c.Value1 = 10; // 触发阈值
            c.Value2 = 3;  // 最大叠加次数
            c.HasCounter = true;
            c.InitialCounter = 0;
            return c;
        }
    }

    [EntityLogic(typeof(ShikigamiBellExhibitDef))]
    public sealed class ShikigamiBellExhibit : Exhibit
    {
        private int _hpAtTurnStart;

        protected override void OnEnterBattle()
        {
            this.Counter = 0;
            _hpAtTurnStart = base.Battle.Player.Hp;
            base.ReactBattleEvent(base.Battle.Player.TurnStarted,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnStarted));
            base.ReactBattleEvent(base.Battle.Player.TurnEnded,
                new EventSequencedReactor<UnitEventArgs>(this.OnTurnEnded));
        }

        private IEnumerable<BattleAction> OnTurnStarted(UnitEventArgs args)
        {
            _hpAtTurnStart = base.Battle.Player.Hp;
            yield break;
        }

        private IEnumerable<BattleAction> OnTurnEnded(UnitEventArgs args)
        {
            int lost = _hpAtTurnStart - base.Battle.Player.Hp;
            if (lost >= Config.Value1.Value && this.Counter < Config.Value2.Value)
            {
                this.Counter++;
                base.NotifyActivating();
                yield return new ApplyStatusEffectAction(typeof(Firepower), base.Battle.Player, 1, null, null, null, 0f, false);
            }
        }
    }
}

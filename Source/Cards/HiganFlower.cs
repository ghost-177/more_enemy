using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Cards.Template;
using LBoL.Core;

namespace EternalWinterMod.Cards
{
    // 彼岸花：抽到时受8穿透伤害，然后放逐自身
    public sealed class HiganFlowerDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = 30006;
            cfg.Value1 = 8;
            return cfg;
        }
    }

    [EntityLogic(typeof(HiganFlowerDef))]
    public sealed class HiganFlower : Card
    {
        public override IEnumerable<BattleAction> OnDraw()
        {
            yield return new DamageAction(
                Battle.Player, Battle.Player,
                DamageInfo.HpLose(Value1, false),
                "", GunType.Single);
            yield return new ExileCardAction(this);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
        }
    }
}

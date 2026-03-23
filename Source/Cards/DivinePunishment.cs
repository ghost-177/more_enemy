using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Cards.Template;
using LBoL.Core;
using MadokaMod.Cards.Template;

namespace EternalWinterMod.Cards
{
    // 神罚令：飘忽；打出时失去1点法力
    public sealed class DivinePunishmentDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = CardIndexGenerator.GetUniqueIndex(cfg);
            cfg.Keywords = Keyword.Ethereal;
            return cfg;
        }
    }

    [EntityLogic(typeof(DivinePunishmentDef))]
    public sealed class DivinePunishment : Card
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return new LockRandomTurnManaAction(1);
        }
    }
}

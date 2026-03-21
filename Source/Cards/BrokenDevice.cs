using System.Collections.Generic;
using System.Linq;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Cards.Template;
using MadokaMod.Cards.Template;

namespace EternalWinterMod.Cards
{
    // 故障机关：抽到时随机弃1张牌，然后放逐自身
    public sealed class BrokenDeviceDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = CardIndexGenerator.GetUniqueIndex(cfg);
            return cfg;
        }
    }

    [EntityLogic(typeof(BrokenDeviceDef))]
    public sealed class BrokenDevice : Card
    {
        public override IEnumerable<BattleAction> OnDraw()
        {
            var targets = Battle.HandZone.Where(c => c != this).ToList();
            if (targets.Count > 0)
            {
                int idx = (int)(Battle.GameRun.BattleRng.NextDouble() * targets.Count);
                yield return new DiscardAction(targets[idx]);
            }
            yield return new ExileCardAction(this);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
        }
    }
}

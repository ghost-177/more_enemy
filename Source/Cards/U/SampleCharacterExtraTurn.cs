using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.Cards.Template;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.StatusEffects;
using LBoL.Core.Battle.BattleActions;
using LBoL.EntityLib.Cards;
using LBoL.EntityLib.StatusEffects.ExtraTurn;

namespace SampleCharacterMod.Cards
{
    public sealed class SampleCharacterExtraTurnDef : SampleCharacterCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            CardConfig config = GetCardDefaultConfig();
            config.Colors = new List<ManaColor>() { ManaColor.Blue };
            config.Cost = new ManaGroup() { Any = 4, Blue = 1 };
            config.UpgradedCost = new ManaGroup() { Any = 3, Blue = 1 };
            config.Rarity = Rarity.Rare;

            config.Type = CardType.Skill;
            config.TargetType = TargetType.Nobody;

            //Mana config for the Time Limit
            config.Mana = new ManaGroup() { Any = 1 };

            config.RelativeEffects = new List<string>() { nameof(TimeIsLimited) };
            config.UpgradedRelativeEffects = new List<string>() { nameof(TimeIsLimited) };

            config.Illustrator = "";

            config.Index = CardIndexGenerator.GetUniqueIndex(config);
            return config;
        }
    }
    
    [EntityLogic(typeof(SampleCharacterExtraTurnDef))]
    //TimeStopCards inhehit from LimitedStopTimeCard instead of Card
    public sealed class SampleCharacterExtraTurn : LimitedStopTimeCard
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return base.BuffAction<ExtraTurn>(1, 0, 0, 0, 0.2f);
            yield return new RequestEndPlayerTurnAction();
            if (base.Limited)
            {
                yield return base.DebuffAction<TimeIsLimited>(base.Battle.Player, 1, 0, 0, 0, true, 0.2f);
            }
            yield break;
        }
    }
}



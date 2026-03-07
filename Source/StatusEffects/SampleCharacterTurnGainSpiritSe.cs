using System.Collections.Generic;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;

namespace SampleCharacterMod.StatusEffects
{
    public sealed class SampleCharacterTurnGainSpiritSeDef : SampleCharacterStatusEffectTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            StatusEffectConfig config = GetDefaultStatusEffectConfig();
            config.RelativeEffects = new List<string>() { nameof(Spirit) };
            return config;
        }
    }

    [EntityLogic(typeof(SampleCharacterTurnGainSpiritSeDef))]
    public sealed class SampleCharacterTurnGainSpiritSe : StatusEffect
    {

		protected override void OnAdded(Unit unit)
		{
			base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnStarted, new EventSequencedReactor<UnitEventArgs>(this.OnOwnerTurnStarted));
		}

		private IEnumerable<BattleAction> OnOwnerTurnStarted(UnitEventArgs args)
		{
            //At the start of the Player's turn, gain Spirit.
			yield return BuffAction<Spirit>(base.Level, 0, 0, 0, 0.2f);
            //This is equivalent to:
            //yield return new ApplyStatusEffectAction<SampleCharacterTurnGainSpiritSe>(base.Owner, base.Level, 0, 0, 0, 0.2f);
			yield break;
		}
    }
}
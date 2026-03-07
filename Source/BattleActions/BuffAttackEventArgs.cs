using LBoL.Core;
using LBoL.Core.Cards;

namespace SampleCharacterMod.BattleActions
{
    public class BuffAttackEventArgs : GameEventArgs
	{
		public Card Card { get; internal set; }
        public int Amount { get; internal set; }
		protected override string GetBaseDebugString()
		{
			return "BuffAttack: " + this.Amount;
		}
	}
}
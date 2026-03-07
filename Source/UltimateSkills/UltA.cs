using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.Core;
using LBoLEntitySideloader.Attributes;
using System.Collections.Generic;
using SampleCharacterMod.GunName;
//using SampleCharacterMod.BattleActions;

namespace SampleCharacterMod.SampleCharacterUlt
{
    public sealed class SampleCharacterUltADef : SampleCharacterUltTemplate
    {
        public override UltimateSkillConfig MakeConfig()
        {
            UltimateSkillConfig config = GetDefaulUltConfig();
            config.Damage = 50;
            return config;
        }
    }

    [EntityLogic(typeof(SampleCharacterUltADef))]
    public sealed class SampleCharacterUltA : UltimateSkill
    {
        public SampleCharacterUltA()
        {
            base.TargetType = TargetType.SingleEnemy;
            base.GunName = GunNameID.GetGunFromId(4158);
        }

        protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
        {
			    EnemyUnit enemy = selector.GetEnemy(base.Battle);
			    yield return new DamageAction(base.Owner, enemy, this.Damage, base.GunName, GunType.Single);
                yield break;
        }
    }
}

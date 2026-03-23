using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act2
{
    public sealed class MountainFairyEWDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Normal;

            config.MaxHp = 48;
            config.MaxHpHard = 53;
            config.MaxHpLunatic = 58;

            config.Damage1 = 11;
            config.Damage1Hard = 12;
            config.Damage1Lunatic = 14;

            config.Defend = 11;
            config.DefendHard = 12;
            config.DefendLunatic = 13;

            config.PowerLoot = new MinMax(40, 55);
            config.BluePointLoot = new MinMax(0, 0);

            config.Gun1 = new List<string> { "Instant" };

            return config;
        }
    }
}

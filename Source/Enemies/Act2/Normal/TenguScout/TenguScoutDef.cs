using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act2
{
    public sealed class TenguScoutEWDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Normal;

            config.MaxHp = 55;
            config.MaxHpHard = 60;
            config.MaxHpLunatic = 65;

            config.Damage1 = 10;
            config.Damage1Hard = 11;
            config.Damage1Lunatic = 12;

            config.Defend = 9;
            config.DefendHard = 10;
            config.DefendLunatic = 11;

            config.PowerLoot = new MinMax(40, 55);
            config.BluePointLoot = new MinMax(0, 0);

            config.Gun1 = new List<string> { "Instant" };

            return config;
        }
    }
}

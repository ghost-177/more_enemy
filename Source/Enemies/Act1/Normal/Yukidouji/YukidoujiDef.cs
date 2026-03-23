using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act1
{
    public sealed class YukidoujiEWDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Normal;

            config.MaxHp = 40;
            config.MaxHpHard = 44;
            config.MaxHpLunatic = 48;

            config.Damage1 = 7;
            config.Damage1Hard = 8;
            config.Damage1Lunatic = 9;

            config.Defend = 8;
            config.DefendHard = 9;
            config.DefendLunatic = 10;

            config.PowerLoot = new MinMax(30, 50);
            config.BluePointLoot = new MinMax(0, 0);

            config.Gun1 = new List<string> { "Instant" };

            return config;
        }
    }
}

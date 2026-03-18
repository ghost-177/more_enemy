using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act3
{
    public sealed class HiganbanaWraithEWDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Normal;

            config.MaxHp = 65;
            config.MaxHpHard = 72;
            config.MaxHpLunatic = 80;

            config.Damage1 = 13;
            config.Damage1Hard = 14;
            config.Damage1Lunatic = 16;

            config.Defend = 12;
            config.DefendHard = 13;
            config.DefendLunatic = 15;

            config.PowerLoot = new MinMax(50, 65);
            config.BluePointLoot = new MinMax(0, 0);

            config.Gun1 = new List<string> { GunNameID.GetGunFromId(800) };

            return config;
        }
    }
}

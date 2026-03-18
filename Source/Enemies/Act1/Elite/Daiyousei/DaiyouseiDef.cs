using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act1
{
    public sealed class DaiyouseiDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Elite;

            config.MaxHp = 100;
            config.MaxHpHard = 110;
            config.MaxHpLunatic = 120;

            config.Damage1 = 10;
            config.Damage1Hard = 11;
            config.Damage1Lunatic = 12;

            config.Damage2 = 14;
            config.Damage2Hard = 15;
            config.Damage2Lunatic = 17;

            config.Defend = 12;
            config.DefendHard = 13;
            config.DefendLunatic = 15;

            config.PowerLoot = new MinMax(60, 80);
            config.BluePointLoot = new MinMax(30, 50);

            config.Gun1 = new List<string> { GunNameID.GetGunFromId(800) };
            config.Gun2 = new List<string> { GunNameID.GetGunFromId(800) };

            return config;
        }
    }
}

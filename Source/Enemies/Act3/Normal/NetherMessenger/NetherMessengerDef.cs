using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act3
{
    public sealed class NetherMessengerDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Normal;

            config.MaxHp = 58;
            config.MaxHpHard = 64;
            config.MaxHpLunatic = 70;

            config.Damage1 = 12;
            config.Damage1Hard = 13;
            config.Damage1Lunatic = 15;

            config.Defend = 14;
            config.DefendHard = 15;
            config.DefendLunatic = 17;

            config.PowerLoot = new MinMax(50, 65);
            config.BluePointLoot = new MinMax(0, 0);

            config.Gun1 = new List<string> { GunNameID.GetGunFromId(800) };

            return config;
        }
    }
}

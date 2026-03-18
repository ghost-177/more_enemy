using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies
{
    public sealed class SampleCharacterEnemyUnitDef : SampleCharacterEnemyUnitTemplate
    {
        public override IdContainer GetId() => nameof(EternalWinterMod);

        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            //Whether the boss should be enabled.
            config.IsPreludeOpponent = false;

            //Color(s) of the exhibits the boss can drop (right-most exhibit).
            config.BaseManaColor = new List<ManaColor>() { ManaColor.White };

            config.Type = EnemyType.Boss;

            //Boss properties
            config.MaxHp = 250;
            config.MaxHpHard = 255;
            config.MaxHpLunatic = 260;

            config.Damage1 = 10;
            config.Damage1Hard = 11;
            config.Damage1Lunatic = 12;
            
            config.Damage2 = 15;
            config.Damage2Hard = 16;
            config.Damage2Lunatic = 17;
            
            config.Damage3 = 20;
            config.Damage3Hard = 21;
            config.Damage3Lunatic = 22;

            config.Damage4 = 25;
            config.Damage4Hard = 26;            
            config.Damage4Lunatic = 27;

            config.Defend = 10;
            config.DefendHard = 11;
            config.DefendLunatic = 12;
            
            config.Count1 = 1;
            config.Count1Hard = 1;
            config.Count1Lunatic = 2;
            
            config.Count2 = 1;
            config.Count2Hard = 1;
            config.Count2Lunatic = 2;
            
            config.PowerLoot = new MinMax(100, 100);
            config.BluePointLoot = new MinMax(100, 100);

            config.Gun1 = new List<string> { GunNameID.GetGunFromId(800) };
            config.Gun2 = new List<string> { GunNameID.GetGunFromId(800) };
            config.Gun3 = new List<string> { GunNameID.GetGunFromId(800) };
            config.Gun4 = new List<string> { GunNameID.GetGunFromId(800) };

            return config;
        }
    }
}

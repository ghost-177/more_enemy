using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act3
{
    public sealed class YuyukoDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Boss;

            config.MaxHp = 310;
            config.MaxHpHard = 330;
            config.MaxHpLunatic = 350;

            // 幽灵蝶舞（单体）
            config.Damage1 = 16;
            config.Damage1Hard = 18;
            config.Damage1Lunatic = 20;

            // 春死满开（高伤）
            config.Damage2 = 24;
            config.Damage2Hard = 26;
            config.Damage2Lunatic = 29;

            // 幽冥结界（连击，Count1次）
            config.Damage3 = 10;
            config.Damage3Hard = 11;
            config.Damage3Lunatic = 13;
            config.Count1 = 2; // 幽冥结界连击次数

            config.Defend = 20;
            config.DefendHard = 22;
            config.DefendLunatic = 25;

            config.PowerLoot = new MinMax(120, 150);
            config.BluePointLoot = new MinMax(80, 100);

            config.Gun1 = new List<string> { GunNameID.GetGunFromId(800) };
            config.Gun2 = new List<string> { GunNameID.GetGunFromId(800) };

            config.BaseManaColor = new List<ManaColor>() { ManaColor.Black };

            return config;
        }
    }
}

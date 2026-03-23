using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act2
{
    public sealed class SuwakoEWDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.IsPreludeOpponent = false;
            config.BaseManaColor = new List<ManaColor>() { ManaColor.Green };
            config.Type = EnemyType.Boss;

            config.MaxHp = 280;
            config.MaxHpHard = 300;
            config.MaxHpLunatic = 320;

            // 蛙击（普通）
            config.Damage1 = 14;
            config.Damage1Hard = 15;
            config.Damage1Lunatic = 17;

            // 大地震（单次重击）
            config.Damage2 = 20;
            config.Damage2Hard = 22;
            config.Damage2Lunatic = 24;

            // 铁环（×2 连击，复用 Count1）
            config.Damage3 = 11;
            config.Damage3Hard = 12;
            config.Damage3Lunatic = 13;

            config.Count1 = 2;
            config.Count1Hard = 2;
            config.Count1Lunatic = 2;

            config.Defend = 18;
            config.DefendHard = 20;
            config.DefendLunatic = 22;

            config.PowerLoot = new MinMax(100, 100);
            config.BluePointLoot = new MinMax(100, 100);

            config.Gun1 = new List<string> { "Instant" };
            config.Gun2 = new List<string> { "Instant" };
            config.Gun3 = new List<string> { "Instant" };

            return config;
        }
    }
}

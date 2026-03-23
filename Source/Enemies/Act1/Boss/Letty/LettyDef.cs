using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act1
{
    public sealed class LettyEWDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.IsPreludeOpponent = false;
            config.BaseManaColor = new List<ManaColor>() { ManaColor.White };
            config.Type = EnemyType.Boss;

            config.MaxHp = 250;
            config.MaxHpHard = 265;
            config.MaxHpLunatic = 280;

            // 普通冰击
            config.Damage1 = 12;
            config.Damage1Hard = 13;
            config.Damage1Lunatic = 14;

            // 暴雪（单次重击）
            config.Damage2 = 17;
            config.Damage2Hard = 19;
            config.Damage2Lunatic = 21;

            // 寒霜弹（×2 连击，复用 Count1 槽位）
            config.Damage3 = 9;
            config.Damage3Hard = 10;
            config.Damage3Lunatic = 11;

            config.Count1 = 2;
            config.Count1Hard = 2;
            config.Count1Lunatic = 2;

            config.Defend = 15;
            config.DefendHard = 17;
            config.DefendLunatic = 19;

            config.PowerLoot = new MinMax(100, 100);
            config.BluePointLoot = new MinMax(100, 100);

            config.Gun1 = new List<string> { "Instant" };
            config.Gun2 = new List<string> { "Instant" };
            config.Gun3 = new List<string> { "Instant" };

            return config;
        }
    }
}

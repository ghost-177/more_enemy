using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using SampleCharacterMod.Cards;
using SampleCharacterMod.Exhibits;
using SampleCharacterMod.SampleCharacterUlt;
namespace SampleCharacterMod
{
    public class SampleCharacterLoadouts
    {
        public static string UltimateSkillA = nameof(SampleCharacterUltA);
        public static string UltimateSkillB = nameof(SampleCharacterUltB);

        public static string ExhibitA = nameof(SampleCharacterExhibitA);
        public static string ExhibitB = nameof(SampleCharacterExhibitB);
        public static List<string> DeckA = new List<string>{
            nameof(Shoot),
            nameof(Shoot),
            nameof(Boundary),
            nameof(Boundary),
            nameof(SampleCharacterAttackR),
            nameof(SampleCharacterAttackR), 
            nameof(SampleCharacterBlockU), 
            nameof(SampleCharacterBlockU), 
            nameof(SampleCharacterMultiAttack), 
        };

        public static List<string> DeckB = new List<string>{
            nameof(Shoot),
            nameof(Shoot),
            nameof(Boundary),
            nameof(Boundary),
            nameof(SampleCharacterAttackW),
            nameof(SampleCharacterAttackW), 
            nameof(SampleCharacterBlockB), 
            nameof(SampleCharacterBlockB), 
            nameof(SampleCharacterAoeAttack),
        };

        public static PlayerUnitConfig playerUnitConfig = new PlayerUnitConfig(
            Id: BepinexPlugin.modUniqueID,
            HasHomeName: true,
            ShowOrder: 8, 
            Order: 0,
            UnlockLevel: 0,
            ModleName: "",
            NarrativeColor: "#e58c27",
            IsSelectable: true,
            MaxHp: 80,
            InitialMana: new ManaGroup() { White = 1, Blue = 1, Black = 1, Red = 1, Green = 0, Colorless = 0, Philosophy = 0 },
            InitialMoney: 99,
            InitialPower: 0,
            BasicRingOrder: 0,
            LeftColor: ManaColor.Philosophy,
            RightColor: ManaColor.Colorless,
            UltimateSkillA: SampleCharacterLoadouts.UltimateSkillA,
            UltimateSkillB: SampleCharacterLoadouts.UltimateSkillB,
            ExhibitA: SampleCharacterLoadouts.ExhibitA,
            ExhibitB: SampleCharacterLoadouts.ExhibitB,
            DeckA: SampleCharacterLoadouts.DeckA,
            DeckB: SampleCharacterLoadouts.DeckB,
            DifficultyA: 3,
            DifficultyB: 2
        );
    }
}

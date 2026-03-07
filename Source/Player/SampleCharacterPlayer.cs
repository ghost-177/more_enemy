using Cysharp.Threading.Tasks;
//using DG.Tweening;
using LBoL.ConfigData;
using LBoL.Core.Units;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using UnityEngine;
using SampleCharacterMod.ImageLoader;
using SampleCharacterMod.Localization;
//using SampleCharacterMod.BattleActions;

namespace SampleCharacterMod
{
    public sealed class SampleCharacterModDef : PlayerUnitTemplate
    {        
        public UniTask<Sprite>? LoadSpellPortraitAsync { get; private set; }

        public override IdContainer GetId()
        {
            return BepinexPlugin.modUniqueID;
        }

        public override LocalizationOption LoadLocalization()
        {
            return SampleCharacterLocalization.PlayerUnitBatchLoc.AddEntity(this);
        }

        public override PlayerImages LoadPlayerImages()
        {
            return SampleCharacterImageLoader.LoadPlayerImages(BepinexPlugin.playerName);
        }

        public override PlayerUnitConfig MakeConfig()
        {
            return SampleCharacterLoadouts.playerUnitConfig;
        }

        [EntityLogic(typeof(SampleCharacterModDef))]
        public sealed class SampleCharacterMod : PlayerUnit 
        {
        }
    }
}
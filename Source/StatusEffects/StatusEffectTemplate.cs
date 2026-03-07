using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using UnityEngine;
using SampleCharacterMod.ImageLoader;
using SampleCharacterMod.Localization;
using SampleCharacterMod.Config;

namespace SampleCharacterMod.StatusEffects
{
    public class SampleCharacterStatusEffectTemplate : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return SampleCharacterDefaultConfig.DefaultID(this);
        }

        public override LocalizationOption LoadLocalization()
        {
            return SampleCharacterLocalization.StatusEffectsBatchLoc.AddEntity(this);
        }

        public override Sprite LoadSprite()
        {
            return SampleCharacterImageLoader.LoadStatusEffectLoader(status: this);
        }

        public override StatusEffectConfig MakeConfig()
        {
            return GetDefaultStatusEffectConfig();
        }

        public static StatusEffectConfig GetDefaultStatusEffectConfig()
        {
            return SampleCharacterDefaultConfig.DefaultStatusEffectConfig();
        }        
    }
}
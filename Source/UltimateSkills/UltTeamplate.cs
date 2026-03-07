using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using UnityEngine;
using SampleCharacterMod.ImageLoader;
using SampleCharacterMod.Localization;
using SampleCharacterMod.Config;

namespace SampleCharacterMod.SampleCharacterUlt
{
    public class SampleCharacterUltTemplate : UltimateSkillTemplate
    {
        public override IdContainer GetId()
        {
            return SampleCharacterDefaultConfig.DefaultID(this);
        }

        public override LocalizationOption LoadLocalization()
        {
            return SampleCharacterLocalization.UltimateSkillsBatchLoc.AddEntity(this);
        }

        public override Sprite LoadSprite()
        {
            return SampleCharacterImageLoader.LoadUltLoader(ult: this);
        }

        public override UltimateSkillConfig MakeConfig()
        {
            throw new System.NotImplementedException();
        }

        public UltimateSkillConfig GetDefaulUltConfig()
        {
            return SampleCharacterDefaultConfig.DefaultUltConfig();
        }
    }
}
using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using EternalWinterMod.Config;
using EternalWinterMod.Localization;
using EternalWinterMod.ImageLoader;
using UnityEngine;

namespace EternalWinterMod.StatusEffects.Template
{
    /// <summary>
    /// 永冬异变状态效果模板基类。
    /// </summary>
    public abstract class EternalWinterSeTemplate : StatusEffectTemplate
    {
        public override IdContainer GetId() => SampleCharacterDefaultConfig.DefaultID(this);

        public override LocalizationOption LoadLocalization()
            => SampleCharacterLocalization.StatusEffectsBatchLoc.AddEntity(this);

        public override Sprite LoadSprite() => SampleCharacterImageLoader.LoadStatusEffectLoader(this);

        protected StatusEffectConfig GetDefaultConfig() => SampleCharacterDefaultConfig.DefaultStatusEffectConfig();
    }
}

using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using SampleCharacterMod.Config;
using SampleCharacterMod.ImageLoader;
using SampleCharacterMod.Localization;

namespace SampleCharacterMod.Exhibits
{
    public class SampleCharacterExhibitTemplate : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return SampleCharacterDefaultConfig.DefaultID(this);
        }

        public override LocalizationOption LoadLocalization()
        {
            return SampleCharacterLocalization.ExhibitsBatchLoc.AddEntity(this);
        }

        public override ExhibitSprites LoadSprite()
        {
            return SampleCharacterImageLoader.LoadExhibitSprite(exhibit: this);
        }

        public override ExhibitConfig MakeConfig()
        {
            return GetDefaultExhibitConfig();
        }

        public ExhibitConfig GetDefaultExhibitConfig()
        {
            return SampleCharacterDefaultConfig.DefaultExhibitConfig();
        }

    }
}
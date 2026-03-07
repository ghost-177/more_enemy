using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using SampleCharacterMod.Config;
using SampleCharacterMod.ImageLoader;
using SampleCharacterMod.Localization;


namespace SampleCharacterMod.Cards.Template
{
    public abstract class SampleCharacterCardTemplate : CardTemplate
    {
        public override IdContainer GetId()
        {
            return SampleCharacterDefaultConfig.DefaultID(this);
        }

        public override CardImages LoadCardImages()
        {
            return SampleCharacterImageLoader.LoadCardImages(this);
        }

        public override LocalizationOption LoadLocalization()
        {
            return SampleCharacterLocalization.CardsBatchLoc.AddEntity(this);
        }

        public CardConfig GetCardDefaultConfig()
        {
            return SampleCharacterDefaultConfig.CardDefaultConfig();
        }
    }


}



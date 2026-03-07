using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.Utils;
using UnityEngine;
using SampleCharacterMod.Localization;
using LBoL.Presentation;

namespace SampleCharacterMod.model

{
    public sealed class SampleCharacterModel : UnitModelTemplate
    {
        //If an ingame model is load, load the chararacter model, otherwise use DirResources/SampleCharacterModel.png 
        public static bool useInGameModel = BepinexPlugin.useInGameModel;
        public static string model_name = useInGameModel ? BepinexPlugin.modelName : "SampleCharacterModel.png";
        //If a custom model is used, use a custom sprite for the Ultimate animation.
        public static string spellsprite_name = "SampleCharacterStand.png";

        public override IdContainer GetId()
        {
            //return new SampleCharacterPlayerDef().UniqueId;
            return BepinexPlugin.modUniqueID;
        }

        public override LocalizationOption LoadLocalization()
        {
            return SampleCharacterLocalization.UnitModelBatchLoc.AddEntity(this);
        }

        public override ModelOption LoadModelOptions()
        {
            if(useInGameModel) 
            {
                //Load the character's spine.
                return new ModelOption(ResourcesHelper.LoadSpineUnitAsync(model_name));
            }
            
            else
            {
                //Load the custom character's sprite.
                return new ModelOption(ResourceLoader.LoadSpriteAsync(model_name, BepinexPlugin.directorySource, ppu: 565));
            }
        }

        public override UniTask<Sprite> LoadSpellSprite()
        {
            if (useInGameModel)
            {
                //Load the ingame character's portrait for the Ultimate.
                return ResourcesHelper.LoadSpellPortraitAsync(model_name);
            }
            else
            {
                //Load the custom character's portrait.
                return ResourceLoader.LoadSpriteAsync(spellsprite_name, BepinexPlugin.directorySource);
            }
        }

        public override UnitModelConfig MakeConfig()
        {
            if (useInGameModel)
            {
                UnitModelConfig config = UnitModelConfig.FromName(model_name).Copy();
                //Flipping the model is only necessary for enemy portraits. 
                config.Flip = BepinexPlugin.modelIsFlipped;
                return config;
            }
            else 
            {
                UnitModelConfig config = DefaultConfig().Copy();
                config.Flip = BepinexPlugin.modelIsFlipped;
                config.Type = 0;
                config.Offset = new Vector2(0, -0.10f);
                config.HasSpellPortrait = true;
                return config;
            }   
        }
    }
}
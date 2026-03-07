using System;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using SampleCharacterMod.Config;
using SampleCharacterMod.Localization;


namespace SampleCharacterMod.Enemies.Template
{
    public class SampleCharacterEnemyUnitTemplate : EnemyUnitTemplate
    {
        public override IdContainer GetId()
        {
            return SampleCharacterDefaultConfig.DefaultID(this);
        }

        public override EnemyUnitConfig MakeConfig()
        {
            return SampleCharacterDefaultConfig.EnemyUnitDefaultConfig();
        }

        public override LocalizationOption LoadLocalization()
        {
            return SampleCharacterLocalization.EnemiesUnitBatchLoc.AddEntity(this);
        }

        public override Type TemplateType()
        {
            return typeof(EnemyUnitTemplate);
        }

        public EnemyUnitConfig GetEnemyUnitDefaultConfig()
        {
            return SampleCharacterDefaultConfig.EnemyUnitDefaultConfig();
        }


    }
}
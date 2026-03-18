using System;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using EternalWinterMod.Config;
using EternalWinterMod.Localization;


namespace EternalWinterMod.Enemies.Template
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
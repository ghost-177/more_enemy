using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using EternalWinterMod.Config;
using EternalWinterMod.Enemies.Localization;


namespace EternalWinterMod.Enemies.Template
{
    public abstract class SampleCharacterEnemyGroupTemplate : EnemyGroupTemplate
    {
        public override IdContainer GetId()
        {
            return SampleCharacterDefaultConfig.DefaultID(this);
        }

        public override EnemyGroupConfig MakeConfig()
        {
            return SampleCharacterDefaultConfig.EnemyGroupDefaultConfig();
        }

        public EnemyGroupConfig GetEnemyGroupDefaultConfig()
        {
            var config = SampleCharacterDefaultConfig.EnemyGroupDefaultConfig();
            var id = GetId().ToString();
            config.Name = EnemyGroupLocalize.GetGroupName(id, id);
            return config;
        }
    }
}
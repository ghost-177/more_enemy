using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using SampleCharacterMod.Config;


namespace SampleCharacterMod.Enemies.Template
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
            return SampleCharacterDefaultConfig.EnemyGroupDefaultConfig();
        }
    }
}
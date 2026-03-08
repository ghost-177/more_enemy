using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using SampleCharacterMod.Config;

namespace SampleCharacterMod.Adventures
{
    // =====================================================================
    // 事件（Adventure）模板基类
    // 所有自定义事件的 Template 都可以继承此类，以复用默认 ID/Config
    //
    // 注意：AdventureTemplate 不支持 LoadLocalization() 方法，
    //       LBoL 的 Adventure 系统通过 YarnSpinner 对话脚本处理文本，
    //       而非通过 sideloader 的批量本地化机制。
    // =====================================================================
    public abstract class SampleAdventureTemplate : AdventureTemplate
    {
        // 使用类名（去掉末尾的 "Def"）作为 ID，与其他模板保持一致
        public override IdContainer GetId()
        {
            return SampleCharacterDefaultConfig.DefaultID(this);
        }

        // 各 Def 子类可通过 DefaultConfig() 拿到初始默认值再修改
        public AdventureConfig GetDefaultAdventureConfig()
        {
            return DefaultConfig();
        }
    }
}

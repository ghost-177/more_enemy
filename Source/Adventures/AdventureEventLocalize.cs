using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SampleCharacterMod.Adventures
{
    // =====================================================================
    // 事件（Adventure）UI 文本本地化读取器
    //
    // 从与 DLL 同目录下的 Adventures{LangCode}.yaml 文件中读取
    // sideloader 不认识的自定义字段（Options、CardSelectDescription 等）。
    //
    // 文件加载规则（按优先级）：
    //   1. Adventures{当前语言}.yaml（如 AdventuresCn.yaml）—— 暂未实现自动检测
    //   2. AdventuresEn.yaml（英文/默认兜底）
    //
    // 多语言扩展说明：
    //   如需添加中文（或其他语言），只需在 DirResources 目录复制
    //   AdventuresEn.yaml → AdventuresCn.yaml 并翻译其中文本。
    //   修改 LoadYaml() 中的语言检测逻辑即可启用。
    // =====================================================================
    internal static class AdventureEventLocalize
    {
        // YAML 反序列化目标模型
        // YamlDotNet 遇到未匹配字段会报错，需加 IgnoreUnmatchedProperties
        private sealed class EventLocData
        {
            public string Name { get; set; }
            public string HostName { get; set; }
            public string Description { get; set; }
            public List<string> Options { get; set; }
            public string CardSelectDescription { get; set; }
        }

        private static Dictionary<string, EventLocData> _cache = null;
        private static readonly object _lock = new object();

        // ----------------------------------------------------------------
        // 获取事件对话描述文本
        // ----------------------------------------------------------------
        public static string GetDescription(string eventId)
        {
            return GetData(eventId)?.Description;
        }

        // ----------------------------------------------------------------
        // 获取选项按钮文本列表
        //
        // 支持具名占位符：
        //   {money}     → SampleCustomEvent.MoneyReward
        //   {healPct}   → SampleCustomEvent.HealPercent
        //   {cardCount} → SampleCustomEvent.CardOfferCount
        //
        // 参数 values 为可选的额外替换字典，供未来事件扩展使用。
        // ----------------------------------------------------------------
        public static string[] GetOptions(string eventId, Dictionary<string, object> values = null)
        {
            var data = GetData(eventId);
            if (data?.Options == null) return null;

            // 合并默认替换字典 + 调用方传入的额外值
            var replacements = BuildDefaultReplacements();
            if (values != null)
                foreach (var kv in values)
                    replacements[kv.Key] = kv.Value;

            var result = new string[data.Options.Count];
            for (int i = 0; i < data.Options.Count; i++)
                result[i] = ApplyReplacements(data.Options[i], replacements);
            return result;
        }

        // ----------------------------------------------------------------
        // 获取卡牌选择阶段的提示文本
        // ----------------------------------------------------------------
        public static string GetCardSelectDescription(string eventId)
        {
            return GetData(eventId)?.CardSelectDescription;
        }

        // ================================================================
        // 私有实现
        // ================================================================

        private static EventLocData GetData(string eventId)
        {
            EnsureLoaded();
            _cache.TryGetValue(eventId, out var data);
            return data;
        }

        private static void EnsureLoaded()
        {
            if (_cache != null) return;
            lock (_lock)
            {
                if (_cache != null) return;
                _cache = LoadYaml();
            }
        }

        private static Dictionary<string, EventLocData> LoadYaml()
        {
            // DLL 与 YAML 文件在同一目录（PostBuild 将 DirResources/* 复制到 scripts 文件夹）
            string asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // 未来可在此处加入语言检测逻辑：
            // string langCode = DetectGameLanguage(); // 返回 "En" / "Cn" 等
            // 目前固定使用英文文件
            string langCode = "En";

            string path = Path.Combine(asmDir, $"Adventures{langCode}.yaml");
            if (!File.Exists(path))
            {
                BepinexPlugin.log.LogWarning($"[AdventureEventLocalize] 未找到文件：{path}");
                return new Dictionary<string, EventLocData>();
            }

            try
            {
                string yaml = File.ReadAllText(path, System.Text.Encoding.UTF8);
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .IgnoreUnmatchedProperties()
                    .Build();
                var result = deserializer.Deserialize<Dictionary<string, EventLocData>>(yaml);
                BepinexPlugin.log.LogInfo($"[AdventureEventLocalize] 已加载 {path}，共 {result?.Count ?? 0} 个事件");
                return result ?? new Dictionary<string, EventLocData>();
            }
            catch (Exception e)
            {
                BepinexPlugin.log.LogWarning($"[AdventureEventLocalize] 解析失败：{e.Message}");
                return new Dictionary<string, EventLocData>();
            }
        }

        // ----------------------------------------------------------------
        // 构建默认占位符替换字典（含 SampleCustomEvent 的常量值）
        // ----------------------------------------------------------------
        private static Dictionary<string, object> BuildDefaultReplacements()
        {
            return new Dictionary<string, object>
            {
                { "money",     SampleCustomEvent.MoneyReward    },
                { "healPct",   SampleCustomEvent.HealPercent    },
                { "cardCount", SampleCustomEvent.CardOfferCount },
            };
        }

        // ----------------------------------------------------------------
        // 将 {key} 形式的占位符替换为对应值
        // ----------------------------------------------------------------
        private static string ApplyReplacements(string template, Dictionary<string, object> replacements)
        {
            if (string.IsNullOrEmpty(template)) return template;
            foreach (var kv in replacements)
                template = template.Replace("{" + kv.Key + "}", kv.Value?.ToString() ?? "");
            return template;
        }
    }
}

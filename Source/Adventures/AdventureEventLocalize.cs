using LBoL.Core;
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
    // sideloader 不认识的自定义字段（Description、Options、CardSelectDescription 等）。
    //
    // 文件选择规则（与 BatchLocalization 一致）：
    //   1. Adventures{CurrentLocale}.yaml（如 AdventuresZhHans.yaml、AdventuresEn.yaml）
    //      — 使用 LBoL.Core.Localization.CurrentLocale 读取当前游戏语言
    //   2. AdventuresEn.yaml（英文/默认兜底，找不到对应语言时使用）
    //
    // 多语言扩展说明：
    //   如需添加新语言，只需在 DirResources 目录复制
    //   AdventuresEn.yaml → Adventures{Locale}.yaml（如 AdventuresZhHans.yaml）
    //   并翻译其中文本即可——无需修改任何代码。
    //   Locale 枚举值名称：En, ZhHans, ZhHant, Ja, Ru, Es, Pl, Pt, Fr, Tr, Ko, Vi, It, De, Uk, Hu
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
        private static string _loadedLangCode = null;
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
        // 获取卡牌选择阶段的提示文本（IMGUI 降级选卡时使用）
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
            // 每次都检查语言是否变化，变了则重新加载
            string currentLang = LBoL.Core.Localization.CurrentLocale.ToString();
            if (_cache != null && _loadedLangCode == currentLang) return;
            lock (_lock)
            {
                currentLang = LBoL.Core.Localization.CurrentLocale.ToString();
                if (_cache != null && _loadedLangCode == currentLang) return;
                _cache = LoadYaml(currentLang);
                _loadedLangCode = currentLang;
            }
        }

        private static Dictionary<string, EventLocData> LoadYaml(string langCode)
        {
            string asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // 1. 根据传入的语言代码构建文件名
            //    LBoL.Core.Localization.CurrentLocale 返回当前语言（Locale 枚举），
            //    ToString() 得到枚举名，如 "En"、"ZhHans"、"Ja" 等。
            //    与 BatchLocalization.DiscoverAndLoadLocFiles 使用相同的命名规则。
            string path = Path.Combine(asmDir, $"Adventures{langCode}.yaml");

            // 2. 找不到对应语言文件则退回英文兜底
            if (!File.Exists(path))
            {
                if (langCode != "En")
                    BepinexPlugin.log.LogInfo($"[AdventureEventLocalize] 未找到 Adventures{langCode}.yaml，退回 AdventuresEn.yaml");
                path = Path.Combine(asmDir, "AdventuresEn.yaml");
            }

            if (!File.Exists(path))
            {
                BepinexPlugin.log.LogWarning($"[AdventureEventLocalize] 未找到本地化文件：{path}");
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
                BepinexPlugin.log.LogInfo($"[AdventureEventLocalize] 已加载 {Path.GetFileName(path)}，共 {result?.Count ?? 0} 个事件");
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

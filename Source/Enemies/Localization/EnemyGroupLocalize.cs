using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LBoL.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace EternalWinterMod.Enemies.Localization
{
    internal static class EnemyGroupLocalize
    {
        private static Dictionary<string, string> _names;

        public static string GetGroupName(string groupId, string fallback = null)
        {
            EnsureLoaded();
            return _names.TryGetValue(groupId, out var name) ? name : (fallback ?? groupId);
        }

        private static void EnsureLoaded()
        {
            if (_names != null) return;
            _names = new Dictionary<string, string>();

            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var locale = LBoL.Core.Localization.CurrentLocale;
            var localeTag = locale.ToString(); // "En", "ZhHans", etc.

            // Try locale-specific file first, then fall back to En
            var filesToTry = new[] { $"EnemyGroup{localeTag}.yaml", "EnemyGroupEn.yaml" };
            foreach (var fileName in filesToTry)
            {
                var path = Path.Combine(dir, fileName);
                if (!File.Exists(path)) continue;
                try
                {
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(PascalCaseNamingConvention.Instance)
                        .IgnoreUnmatchedProperties()
                        .Build();
                    var data = deserializer.Deserialize<Dictionary<string, EnemyGroupLocData>>(
                        File.ReadAllText(path));
                    if (data != null)
                        foreach (var kv in data)
                            if (!string.IsNullOrEmpty(kv.Value?.Name))
                                _names[kv.Key] = kv.Value.Name;
                    break;
                }
                catch { }
            }
        }

        private class EnemyGroupLocData
        {
            public string Name { get; set; }
        }
    }
}

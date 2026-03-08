using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LBoL.Base;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoL.Presentation;
using SampleCharacterMod.Adventures;
using SampleCharacterMod.Cards.Template;
using SampleCharacterMod.Config;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


namespace SampleCharacterMod
{
    [BepInPlugin(SampleCharacterMod.PInfo.GUID, SampleCharacterMod.PInfo.Name, SampleCharacterMod.PInfo.version)]
    [BepInDependency(LBoLEntitySideloader.PluginInfo.GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(AddWatermark.API.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {
        //The Unique mod ID of the mod.
        //If defined, this is also the ID used by the Act 1 boss.
        //WARNING: It is mandatory to rename it to avoid issues.
        public static string modUniqueID = "SampleCharacterMod";
        //Name of the character.
        //This is also the prefix that is used before every .png file in DirResources. 
        public static string playerName = "SampleCharacter";
        //Whether to us an ingame or custom model.
        //InGame: Will load the character model of the ingame character.
        //Custom: Will load DirResource/SampleCharacterModel.png 
        public static bool useInGameModel = true;
        //If InGame is selected, this is the model that will be loaded. 
        //Check LBoL.EntityLib.EnemyUnits.Character or using LBoL.EntityLib.PlayerUnits for a list of all the characters available. 
        public static string modelName = nameof(Youmu);
        //Some in-game model needs to be flipped (most notably elites).
        public static bool modelIsFlipped = true;
        //The character's off-color.
        //Used to separate cards in the card collection and put the off-color cards at the end.
        public static List<ManaColor> offColors = new List<ManaColor>() { ManaColor.Colorless };

        //Whether the Act 1 boss should be enabled.
        //The value can be customized LBoL/BepInEx/config/
        public static ConfigEntry<bool> enableAct1Boss;

        public static CustomConfigEntry<bool> enableAct1BossEntry = new CustomConfigEntry<bool>(
            value: false,
            section: "EnableAct1Boss",
            key: "EnableAct1Boss",
            description: "Toggle the Act 1 boss. Default: Off");

        private static readonly Harmony harmony = SampleCharacterMod.PInfo.harmony;

        internal static BepInEx.Logging.ManualLogSource log;

        internal static TemplateSequenceTable sequenceTable = new TemplateSequenceTable();

        internal static IResourceSource embeddedSource = new EmbeddedSource(Assembly.GetExecutingAssembly());

        // add this for audio loading
        internal static DirectorySource directorySource = new DirectorySource(SampleCharacterMod.PInfo.GUID, "");


        private void Awake()
        {
            log = Logger;
            ///Load the custom config entry.
            enableAct1Boss = Config.Bind(enableAct1BossEntry.Section, enableAct1BossEntry.Key, enableAct1BossEntry.Value, enableAct1BossEntry.Description);

            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            CardIndexGenerator.PromiseClearIndexSet();
            EntityManager.RegisterSelf();

            harmony.PatchAll();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(AddWatermark.API.GUID))
                WatermarkWrapper.ActivateWatermark();

            Func<Sprite> getSprite = () => ResourceLoader.LoadSprite("BossIcon.png", directorySource);
            EnemyUnitTemplate.AddBossNodeIcon(nameof(SampleCharacterMod.Enemies.SampleCharacterMod), getSprite);
        }

        // -----------------------------------------------------------------
        // 测试快捷键：F6
        //
        // 在游戏进行中按下 F6，将以极高权重把 SampleCustomEvent 注入
        // 当前幕的事件池，下一个 Adventure 节点必定触发自定义事件。
        //
        // 使用方法：
        //   1. 开始游戏并进入地图
        //   2. 按 F6（控制台会打印确认信息）
        //   3. 走到任意 Adventure（事件）节点
        //   4. 查看 BepInEx 控制台的日志输出验证事件触发
        // -----------------------------------------------------------------
        // -----------------------------------------------------------------
        // 自定义事件选择界面（IMGUI）
        //
        // 由 SampleAdventureDialogPatch 的 CustomAdventureFlow 调用：
        //   - 设置 pendingChoiceOptions 触发显示
        //   - 等待 pendingChoiceResult >= 0 表示玩家做出选择
        //   - 清除 pendingChoiceOptions 隐藏界面
        //
        // 背景图：pendingChoiceBackground 不为 null 时先绘制背景
        // 描述文：pendingChoiceDescription 不为 null/空时绘制事件描述
        // -----------------------------------------------------------------
        internal static volatile string[] pendingChoiceOptions = null;
        internal static volatile int pendingChoiceResult = -1;
        internal static Texture2D pendingChoiceBackground = null;
        internal static string pendingChoiceDescription = null;

        private void OnGUI()
        {
            if (pendingChoiceOptions == null) return;

            float sw = Screen.width;
            float sh = Screen.height;

            // 背景图（若已加载）
            if (pendingChoiceBackground != null)
            {
                GUI.DrawTexture(new Rect(0, 0, sw, sh), pendingChoiceBackground, ScaleMode.ScaleAndCrop);
            }

            // 半透明遮罩，提升文字可读性
            GUI.color = new Color(0f, 0f, 0f, 0.65f);
            GUI.DrawTexture(new Rect(0, 0, sw, sh), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // 事件描述文本
            if (!string.IsNullOrEmpty(pendingChoiceDescription))
            {
                var descStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 20,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                };
                descStyle.normal.textColor = new Color(1f, 0.95f, 0.8f);
                float descW = sw * 0.75f;
                float descH = 140f;
                float descX = (sw - descW) * 0.5f;
                float descY = sh * 0.18f;
                GUI.Label(new Rect(descX, descY, descW, descH), pendingChoiceDescription, descStyle);
            }

            // 选项按钮
            var options = pendingChoiceOptions; // 防止中途被置 null 导致长度变化
            if (options == null) return;

            float btnW = Mathf.Min(620f, sw * 0.7f);
            float btnH = 62f;
            float gap  = 16f;
            float totalH = options.Length * btnH + (options.Length - 1) * gap;
            float startY = (sh - totalH) * 0.5f + sh * 0.08f; // 稍微偏下，给描述留空间
            float startX = (sw - btnW) * 0.5f;

            var btnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 21,
                alignment = TextAnchor.MiddleCenter,
            };

            for (int i = 0; i < options.Length; i++)
            {
                if (GUI.Button(new Rect(startX, startY + i * (btnH + gap), btnW, btnH), options[i], btnStyle))
                {
                    pendingChoiceResult = i;
                    pendingChoiceOptions = null; // 隐藏界面
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                // 通过 GameMaster（Presentation 层）获取当前 GameRunController
                var master = UnityEngine.Object.FindObjectOfType<LBoL.Presentation.GameMaster>();
                var gameRun = master?.CurrentGameRun;
                if (gameRun != null)
                {
                    SampleAdventureDebugHelper.ForceNextAdventure(gameRun);
                }
                else
                {
                    log.LogWarning("[SampleCustomEvent] F6 按下但当前没有进行中的游戏。");
                }
            }
        }

        private void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchSelf();
        }
    }
}

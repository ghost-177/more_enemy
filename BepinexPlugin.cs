using BepInEx;
using HarmonyLib;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoL.Presentation;
using System.Reflection;
using UnityEngine;


namespace EternalWinterMod
{
    [BepInPlugin(PInfo.GUID, PInfo.Name, PInfo.version)]
    [BepInDependency(LBoLEntitySideloader.PluginInfo.GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(AddWatermark.API.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {
        // 永冬异变 EnemyMod 唯一标识，用作 Owner 字段
        public static string modUniqueID = "EternalWinterMod";

        private static readonly Harmony harmony = PInfo.harmony;

        internal static BepInEx.Logging.ManualLogSource log;

        internal static TemplateSequenceTable sequenceTable = new TemplateSequenceTable();

        internal static IResourceSource embeddedSource = new EmbeddedSource(Assembly.GetExecutingAssembly());

        internal static DirectorySource directorySource = new DirectorySource(PInfo.GUID, "");


        private void Awake()
        {
            log = Logger;

            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            EntityManager.RegisterSelf();
            harmony.PatchAll();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(AddWatermark.API.GUID))
                WatermarkWrapper.ActivateWatermark();
        }

        // -----------------------------------------------------------------
        // 自定义事件选择界面（IMGUI）
        // -----------------------------------------------------------------
        internal static volatile string[] pendingChoiceOptions = null;
        internal static volatile int pendingChoiceResult = -1;
        internal static Texture2D pendingChoiceBackground = null;
        internal static string pendingChoiceDescription = null;

        // ---- IMGUI 按钮纹理（圆角，延迟初始化） ----
        private static Texture2D _btnNormalTex;
        private static Texture2D _btnHoverTex;
        private static Texture2D _btnActiveTex;

        // 生成 32×32 圆角矩形纹理（radius=10），配合 border=(10,10,10,10) 做 9-slice
        private static Texture2D MakeRoundedTex(Color col)
        {
            const int size = 32, radius = 10;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(0, Mathf.Max(radius - x - 0.5f, x - (size - 1 - radius) + 0.5f));
                    float dy = Mathf.Max(0, Mathf.Max(radius - y - 0.5f, y - (size - 1 - radius) + 0.5f));
                    pixels[y * size + x] = Mathf.Sqrt(dx * dx + dy * dy) <= radius
                        ? new Color(col.r, col.g, col.b, col.a)
                        : Color.clear;
                }
            }
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static void EnsureUITextures()
        {
            if (_btnNormalTex != null) return;
            _btnNormalTex = MakeRoundedTex(new Color(0.07f, 0.05f, 0.02f, 0.90f));
            _btnHoverTex  = MakeRoundedTex(new Color(0.28f, 0.20f, 0.04f, 0.95f));
            _btnActiveTex = MakeRoundedTex(new Color(0.50f, 0.35f, 0.05f, 1.00f));
        }

        private void OnGUI()
        {
            if (pendingChoiceOptions == null) return;

            GUI.depth = 5;
            EnsureUITextures();

            float sw = Screen.width;
            float sh = Screen.height;
            float leftW = sw * 0.5f;

            // ---- 右半屏：背景图（StretchToFill 拉伸填满指定区域） ----
            float rightX = sw * 0.6f;
            float rightW = sw * 0.3f;
            float rightY = sh * 0.25f;
            float rightH = sh * 0.4f;
            if (pendingChoiceBackground != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(rightX, rightY, rightW, rightH), pendingChoiceBackground, ScaleMode.StretchToFill);
            }

            GUI.color = Color.white;

            // ---- 左半屏：选项按钮布局 ----
            var options = pendingChoiceOptions;
            if (options == null) return;

            float btnW = Mathf.Min(leftW - 60f, 520f);
            float btnH = 58f;
            float gap  = 12f;
            float totalBtnH = options.Length * btnH + (options.Length - 1) * gap;
            float panelPadding = 36f;

            float btnsStartY = sh - panelPadding - totalBtnH;
            float btnsStartX = (leftW - btnW) * 0.5f;

            // ---- 描述文本 ----
            if (!string.IsNullOrEmpty(pendingChoiceDescription))
            {
                var descStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 18,
                    alignment = TextAnchor.LowerLeft,
                    wordWrap = true,
                };
                descStyle.normal.textColor = new Color(1f, 0.93f, 0.75f, 1f);
                float descH = 120f;
                float descY = btnsStartY - descH - 20f;
                GUI.Label(new Rect(btnsStartX, descY, btnW, descH), pendingChoiceDescription, descStyle);
            }

            // ---- 选项按钮（圆角，深色背景+金色文字） ----
            var btnStyle = new GUIStyle
            {
                fontSize = 19,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(18, 12, 0, 0),
            };
            btnStyle.normal.background = _btnNormalTex;
            btnStyle.normal.textColor  = new Color(0.96f, 0.84f, 0.45f, 1f);
            btnStyle.hover.background  = _btnHoverTex;
            btnStyle.hover.textColor   = new Color(1.00f, 0.95f, 0.65f, 1f);
            btnStyle.active.background = _btnActiveTex;
            btnStyle.active.textColor  = Color.white;
            btnStyle.border            = new RectOffset(10, 10, 10, 10);

            for (int i = 0; i < options.Length; i++)
            {
                float btnY = btnsStartY + i * (btnH + gap);
                if (GUI.Button(new Rect(btnsStartX, btnY, btnW, btnH), options[i], btnStyle))
                {
                    pendingChoiceResult = i;
                    pendingChoiceOptions = null;
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                var master = UnityEngine.Object.FindObjectOfType<GameMaster>();
                var gameRun = master?.CurrentGameRun;
                if (gameRun != null)
                    log.LogInfo("[EternalWinter] F6 pressed. GameRun active.");
                else
                    log.LogWarning("[EternalWinter] F6 pressed but no active GameRun.");
            }
        }

        private void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchSelf();
        }
    }
}

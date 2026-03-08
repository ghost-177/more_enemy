# LBoL Sample Character Mod — 项目说明（给 Claude 看的）

## 项目基本信息

- **项目目录**：`E:\extraTool\VisualStudio2022\vsProject\lbol_sample_character_mod-main`
- **游戏目录**：`D:\Program FIles\Steam\steamapps\common\LBoL`（注意路径中有空格）
- **游戏 DLL**：`$(GameFolder)\LBoL_Data\Managed\`
- **BepInEx 目录**：`$(GameFolder)\BepInEx\`
- **Sideloader 插件**：`$(BepInExFolder)\plugins\LBoL-Entity-Sideloader\`

## 技术栈

| 组件 | 版本/说明 |
|---|---|
| 语言 | C# / .NET Standard 2.1 |
| Mod 框架 | BepInEx 5 |
| 补丁框架 | HarmonyX 2.9 |
| 实体注册 | LBoLEntitySideloader |
| 访问私有成员 | Krafs.Publicizer 2.2.1 |

## 项目构建

```
dotnet build SampleCharacterMod_windows.csproj
```

构建后会自动 copy DLL 到 `$(BepInExFolder)\scripts\SampleCharacterMod_windows\`。

**已知无害警告**（不影响构建）：
- `MSB3245: mcs.dll 未找到` — libs/mcs.dll 不存在，可忽略
- `CS0436: IgnoresAccessChecksToAttribute 冲突` — Krafs.Publicizer 与 MonoMod 的重复定义，可忽略

## 入口点

`BepinexPlugin.cs` → `Awake()`:
1. `EntityManager.RegisterSelf()` — 注册所有实体（Card/Exhibit/Adventure/Enemy 等）
2. `harmony.PatchAll()` — 应用所有 Harmony 补丁

## 实体架构（Template + Logic 双层）

所有实体都分两个类：

```
DefClass : XxxTemplate   →  提供 ID + Config（外观/配置）
LogicClass : XxxBase     →  [EntityLogic(typeof(DefClass))] 绑定到 Def
```

ID 规则：`SampleCharacterDefaultConfig.DefaultID(this)` 用类名（自动去掉末尾"Def"）作为 ID。

### 实体类型对照表

| Template 基类 | Logic 基类 | Config 类 | 本地化文件 |
|---|---|---|---|
| `CardTemplate` | `Card` | `CardConfig` | `DirResources/CardsEn.yaml` |
| `ExhibitTemplate` | `Exhibit` | `ExhibitConfig` | `DirResources/ExhibitsEn.yaml` |
| `AdventureTemplate` | `Adventure` | `AdventureConfig` | `DirResources/AdventuresEn.yaml` |
| `StatusEffectTemplate` | `StatusEffect` | `StatusEffectConfig` | `DirResources/StatusEffectsEn.yaml` |
| `EnemyUnitTemplate` | `EnemyUnit` | `EnemyUnitConfig` | `DirResources/EnemyUnitEn.yaml` |

## 事件（Adventure）系统详解

### 两种地图节点的区别（重要！）

| 节点类型 | Station 类 | 说明 |
|---|---|---|
| Adventure（?）节点 | `AdventureStation` | 触发对话/剧情事件，本文件实现的就是这个 |
| Gap（营地）节点 | `GapStation` | 提供休息/升级选项（GapOption），完全不同的系统 |

**`GapOptions` 属于 `GapStation`，Adventure 类没有此属性，不要混淆。**

### Adventure 的两个层次

```csharp
// 1. Def/Template 类：提供 ID 和 AdventureConfig
public sealed class SampleCustomEventDef : SampleAdventureTemplate
{
    public override AdventureConfig MakeConfig()
    {
        var config = GetDefaultAdventureConfig();
        config.HostId = "";   // NPC 角色 ID，留空=无
        config.HostId2 = "";  // 第二 NPC
        config.Music = 0;     // BGM，0=地图默认
        config.HideUlt = false;
        return config;
    }
}

// 2. Logic 类：事件逻辑
[AdventureInfo(WeighterType = typeof(SampleCustomEventWeighter))]  // 注册权重控制器
[EntityLogic(typeof(SampleCustomEventDef))]                         // 绑定 Def
public sealed class SampleCustomEvent : Adventure
{
    public const int MoneyReward = 80;
    public const int HealPercent = 25;

    // 权重控制器建议作为嵌套类
    public sealed class SampleCustomEventWeighter : IAdventureWeighter
    {
        public float WeightFor(Type adventureType, GameRunController gameRun)
        {
            if (adventureType != typeof(SampleCustomEvent)) return 0f;
            int stage = gameRun.CurrentStage?.Level ?? 0;
            return (stage == 1 || stage == 2) ? 1f : 0f;
        }
    }
}
```

### YarnSpinner 问题及绕过方案

LBoL 的 Adventure 依赖预编译的 YarnSpinner `.yarn` 脚本。Mod 无法方便地添加这些文件。

**绕过方案（三个 Harmony 补丁，见 `Source/Adventures/SampleAdventureDialogPatch.cs`）：**

> **重要：sideloader 的 AdventureTemplate.Consume 不会把 AdventureConfig 注册进 `AdventureConfig._IdTable`！**
> 必须在补丁 1 里手动注册，否则 `Adventure.Initialize()` 会抛出
> `InvalidDataException: Cannot find adventure config for <SampleCustomEvent>`

#### 补丁 1：注入 AdventureConfig 到 _IdTable + 注入事件到 AdventurePool

```csharp
[HarmonyPatch]
internal static class Stage_Initialize_Patch
{
    [HarmonyTargetMethod]
    static MethodBase TargetMethod() => AccessTools.Method(typeof(Stage), "Initialize");

    static bool _configRegistered = false;

    [HarmonyPostfix]
    static void Postfix(Stage __instance)
    {
        // 必须先注册 config，再注入 pool！
        EnsureAdventureConfigRegistered();
        __instance.AdventurePool?.Add(typeof(SampleCustomEvent), 1f);
    }

    static void EnsureAdventureConfigRegistered()
    {
        if (_configRegistered) return;
        _configRegistered = true;

        // AdventureConfig._IdTable: static Dictionary<string, AdventureConfig>
        // Adventure.Initialize() 从这里查 config，找不到就崩溃
        // sideloader 不会自动注册，必须手动加
        var idTableField = AccessTools.Field(typeof(AdventureConfig), "_IdTable");
        var table = (Dictionary<string, AdventureConfig>)idTableField.GetValue(null);
        if (table != null && !table.ContainsKey("SampleCustomEvent"))
        {
            table["SampleCustomEvent"] = new AdventureConfig(
                No: 9001, Id: "SampleCustomEvent",
                HostId: "", HostId2: "", Music: 0, HideUlt: false, TempArt: false);
        }
    }
}
```

#### 补丁 2：拦截 GameMaster.AdventureFlow（正确方式）

> **架构关键：AdventureStation.OnEnter() 是 Core 层通知，不控制对话！**
> 对话流程由 Presentation 层的 `GameMaster.AdventureFlow(Station)` 控制。
> 必须拦截 AdventureFlow 才能绕过 VnPanel 的 YarnSpinner 对话加载。

```csharp
[HarmonyPatch]
internal static class GameMaster_AdventureFlow_Patch
{
    [HarmonyTargetMethod]
    static MethodBase TargetMethod() => AccessTools.Method(typeof(GameMaster), "AdventureFlow");

    [HarmonyPrefix]
    static bool Prefix(GameMaster __instance, Station station, ref IEnumerator __result)
    {
        if (!(station is AdventureStation advStation)) return true;
        if (!(advStation.Adventure is SampleCustomEvent customAdv)) return true;

        __result = CustomAdventureFlow(advStation, customAdv);
        return false; // 跳过原始 AdventureFlow（不启动 VnPanel 对话）
    }

    static IEnumerator CustomAdventureFlow(AdventureStation station, SampleCustomEvent adventure)
    {
        // 发放奖励...
        yield break; // 协程结束，外层 CoEnterStation 自动调用 EndStationFlow 处理离站
    }
}
```

**必须手动调用 `station.Finish()`**，否则地图下一节点不会解锁（按钮 inactive）。
`CoEnterStation` 的自动处理依赖原生 AdventureFlow 内部的完成信号，完全替换后需要自己触发：

```csharp
// 在 CustomAdventureFlow 最后调用
station.Finish();
```

### GameMaster 官方自定义 Adventure 机制（可选替代方案）

```
GameMaster.RegisterExtraAdventureHandlers<TAdventure>(IAdventureHandler handler)
IAdventureHandler 接口：
  - EnterAdventure(Adventure adventure)
  - LeaveAdventure(Adventure adventure)
```

可以在 `GameMaster.Awake()` 的 Postfix 中注册，作为 AdventureFlow 补丁的替代方案。

### DoNotPublicize 中需要用 AccessTools 的成员

以下成员在 csproj 的 DoNotPublicize 列表中，**不能直接调用，必须通过 AccessTools**：

- `Stage.Initialize` → `AccessTools.Method(typeof(Stage), "Initialize")`
- `Adventure.DialogName` → `AccessTools.PropertyGetter(typeof(Adventure), "DialogName")`
- `Adventure.InitVariables` → `AccessTools.Method(typeof(Adventure), "InitVariables")`

### Stage.AdventurePool

- 类型：`UniqueRandomPool<Type>`
- 添加方法：`pool.Add(typeof(MyAdventure), weight)`
- 在 `Stage.Initialize` 的 Postfix 中注入

### GameMaster / GameRunController 访问

```csharp
// GameMaster 在 LBoL.Presentation 命名空间，非静态，需要 FindObjectOfType
var master = UnityEngine.Object.FindObjectOfType<LBoL.Presentation.GameMaster>();
var gameRun = master?.CurrentGameRun;
```

### FakeAdventure 注意事项

`FakeAdventure` 是 `sealed` 类，**不能继承**。自定义事件直接继承 `Adventure`。

## 测试快捷键（F6）

在 `BepinexPlugin.Update()` 中：
- 按 **F6** → 以权重 9999 向当前幕 AdventurePool 注入事件
- 下一个 Adventure（?）节点必定触发自定义事件
- 查看 BepInEx 控制台日志确认奖励

```csharp
private void Update()
{
    if (Input.GetKeyDown(KeyCode.F6))  // 需要引用 UnityEngine.InputLegacyModule.dll
    {
        var master = UnityEngine.Object.FindObjectOfType<LBoL.Presentation.GameMaster>();
        var gameRun = master?.CurrentGameRun;
        if (gameRun != null)
            SampleAdventureDebugHelper.ForceNextAdventure(gameRun);
    }
}
```

## 本地化

Adventure 本地化文件：`DirResources/AdventuresEn.yaml`

```yaml
SampleCustomEventDef:
  Name: "The Mysterious Traveler's Gift"
  Description: |-
    A mysterious traveler appears before you on the road...
```

注意：Adventure 本地化**不通过** `BatchLocalization`，走 sideloader 的 AdventuresEn.yaml 机制。

## 关键 using 命名空间

```csharp
using LBoL.Core;                    // GameRunController, Stage
using LBoL.Core.Adventures;         // Adventure, IAdventureWeighter, AdventureInfoAttribute
using LBoL.Core.Stations;           // AdventureStation, GapStation
using LBoL.ConfigData;              // AdventureConfig
using LBoL.Presentation;            // GameMaster
using LBoLEntitySideloader;         // EntityManager, IdContainer
using LBoLEntitySideloader.Entities;// AdventureTemplate, CardTemplate, etc.
using LBoLEntitySideloader.Attributes; // AdventureInfo (属性所在命名空间)
using HarmonyLib;                   // HarmonyPatch, AccessTools
```

## csproj 关键引用

需要显式引用才能编译的 DLL（不在 NuGet 中，需要 HintPath 指向游戏目录）：

```xml
<Reference Include="UnityEngine.InputLegacyModule">  <!-- Input.GetKeyDown -->
    <HintPath>$(GameFolder)\LBoL_Data\Managed\UnityEngine.InputLegacyModule.dll</HintPath>
</Reference>
<Reference Include="LBoL.Core">
    <HintPath>$(GameFolder)\LBoL_Data\Managed\LBoL.Core.dll</HintPath>
</Reference>
<Reference Include="LBoL.Presentation">
    <HintPath>$(GameFolder)\LBoL_Data\Managed\LBoL.Presentation.dll</HintPath>
</Reference>
```

## 文件结构

```
lbol_sample_character_mod-main/
├── BepinexPlugin.cs                    # 入口点，F6 测试快捷键
├── SampleCharacterMod_windows.csproj   # 项目配置，Publicize/DoNotPublicize 列表
├── Source/
│   ├── Adventures/
│   │   ├── SampleAdventureTemplate.cs  # Adventure Def 基类
│   │   ├── SampleCustomEvent.cs        # 自定义事件 Def + Logic + Weighter
│   │   └── SampleAdventureDialogPatch.cs # 3个 Harmony 补丁
│   ├── Cards/                          # 卡牌相关
│   ├── Exhibits/                       # 遗物相关
│   ├── Enemies/                        # 敌人相关
│   └── Localization/
│       └── Localization.cs             # BatchLocalization（卡牌/遗物用）
├── DirResources/
│   ├── AdventuresEn.yaml               # 事件本地化
│   ├── CardsEn.yaml                    # 卡牌本地化
│   └── ...
└── Resources/                          # 嵌入图片资源（.png）
```

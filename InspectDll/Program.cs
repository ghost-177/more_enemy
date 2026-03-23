using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Linq;

var sideloaderPath = @"D:\software\steam\steamapps\workshop\content\1140150\3572815053\LBoL-Entity-Sideloader.dll";
var sideloader = ModuleDefMD.Load(sideloaderPath);

var egt = sideloader.GetTypes().FirstOrDefault(t => t.Name == "EnemyGroupTemplate");

// Find all sideloader code that calls LoadLocalization
Console.WriteLine("=== Sideloader methods that call LoadLocalization ===");
foreach (var t in sideloader.GetTypes())
    foreach (var m in t.Methods.Where(m => m.Body != null))
        foreach (var instr in m.Body.Instructions)
        {
            var op = instr.Operand?.ToString() ?? "";
            if (op.Contains("LoadLocalization"))
                Console.WriteLine($"  {t.Name}.{m.Name}: {instr}");
        }

// EntityDefinition.ProcessLocalization IL
var edType2 = sideloader.GetTypes().FirstOrDefault(t => t.Name == "EntityDefinition");
Console.WriteLine("=== EntityDefinition.ProcessLocalization ===");
var pl = edType2?.Methods.FirstOrDefault(x => x.Name == "ProcessLocalization");
if (pl?.Body != null)
    foreach (var instr in pl.Body.Instructions)
        Console.WriteLine($"  {instr}");
else
    Console.WriteLine("  not found or no body");

// EnemyUnitTemplate - check if LoadLocalization is virtual/abstract
var eut4 = sideloader.GetTypes().FirstOrDefault(t => t.Name == "EnemyUnitTemplate");
var ll_eut4 = eut4?.Methods.FirstOrDefault(x => x.Name == "LoadLocalization");
Console.WriteLine($"\nEnemyUnitTemplate.LoadLocalization IsVirtual={ll_eut4?.IsVirtual} IsAbstract={ll_eut4?.IsAbstract}");

var corePath4 = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.Core.dll";
var core4 = ModuleDefMD.Load(corePath4);
var locType = core4.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Localization");
Console.WriteLine($"LBoL.Core.Localization: {locType?.FullName}");
Console.WriteLine("  IsAbstract: " + locType?.IsAbstract);
Console.WriteLine("  IsSealed: " + locType?.IsSealed);
Console.WriteLine("=== Localization properties ===");
if (locType != null)
    foreach (var p in locType.Properties)
        Console.WriteLine($"  {(locType.Fields.FirstOrDefault(f => f.Name.Contains(p.Name) && f.IsStatic) != null ? "static " : "")}{p.PropertySig?.RetType} {p.Name}");
Console.WriteLine("=== Localization fields ===");
if (locType != null)
    foreach (var f in locType.Fields)
        Console.WriteLine($"  {f.Name}: {f.FieldType}");

Console.WriteLine("=== EnemyGroupTemplate all methods (full sigs) ===");
if (egt != null)
    foreach (var m in egt.Methods)
        Console.WriteLine($"  {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => $"{p.Type}"))})");

// Check EntityDefinition for LoadLocalization or Consume
var edType = sideloader.GetTypes().FirstOrDefault(t => t.Name == "EntityDefinition");
Console.WriteLine("\n=== EntityDefinition all methods ===");
if (edType != null)
    foreach (var m in edType.Methods)
        Console.WriteLine($"  {m.ReturnType} {m.Name}");

// LBoL.Core.LocaleExtensions methods
var corePath3 = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.Core.dll";
var core3 = ModuleDefMD.Load(corePath3);
var leType = core3.GetTypes().FirstOrDefault(t => t.Name == "LocaleExtensions");
Console.WriteLine("\n=== LocaleExtensions methods ===");
if (leType != null)
    foreach (var m in leType.Methods.Where(m => !m.IsConstructor))
        Console.WriteLine($"  {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => $"{p.Type}"))})");

// Find classes that have CurrentLocale or GetLocale or Locale property
Console.WriteLine("\n=== Core types with static Locale getter ===");
foreach (var t in core3.GetTypes())
    foreach (var p in t.Properties.Where(p => p.Name.Contains("Locale") || p.Name.Contains("Language")))
        Console.WriteLine($"  {t.FullName}.{p.Name}: {p.PropertySig?.RetType}");

// LocalizationOption constructors and all methods
var loType2 = sideloader.GetTypes().FirstOrDefault(t => t.Name == "LocalizationOption");
Console.WriteLine("=== LocalizationOption constructors ===");
if (loType2 != null)
    foreach (var m in loType2.Methods.Where(m => m.IsConstructor))
    {
        Console.WriteLine($"  .ctor({string.Join(", ", m.Parameters.Skip(1).Select(p => $"{p.Type} {p.Name}"))})");
    }
Console.WriteLine("=== LocalizationOption FillLocalizationTables ===");
var flt = loType2?.Methods.FirstOrDefault(x => x.Name == "FillLocalizationTables");
if (flt?.Body != null)
    foreach (var instr in flt.Body.Instructions)
        Console.WriteLine($"  {instr}");
Console.WriteLine("=== LocalizationOption FillUnitNameTable ===");
var funt = loType2?.Methods.FirstOrDefault(x => x.Name == "FillUnitNameTable");
Console.WriteLine($"  Sig: {funt?.MethodSig}");
if (funt != null)
    foreach (var p in funt.Parameters.Skip(1))
        Console.WriteLine($"  Param: {p.Name}: {p.Type}");

// EnemyUnitTemplate - all fields and their types
var eut3 = sideloader.GetTypes().FirstOrDefault(t => t.Name == "EnemyUnitTemplate");
Console.WriteLine("\n=== EnemyUnitTemplate fields ===");
if (eut3 != null)
    foreach (var f in eut3.Fields)
        Console.WriteLine($"  {f.Name}: {f.FieldType}");

// LocalizationFiles type
var lfType = sideloader.GetTypes().FirstOrDefault(t => t.Name == "LocalizationFiles");
Console.WriteLine("\n=== LocalizationFiles methods ===");
if (lfType != null)
    foreach (var m in lfType.Methods)
        Console.WriteLine($"  {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => $"{p.Type}"))})");


// EnemyUnitTemplate.LoadLocalization - signature
var eut2 = sideloader.GetTypes().FirstOrDefault(t => t.Name == "EnemyUnitTemplate");
var ll_eut = eut2?.Methods.FirstOrDefault(x => x.Name == "LoadLocalization");
Console.WriteLine($"EnemyUnitTemplate.LoadLocalization sig: {ll_eut?.MethodSig}");
Console.WriteLine($"  RetType: {ll_eut?.ReturnType}");
if (ll_eut != null)
    foreach (var p in ll_eut.Parameters)
        Console.WriteLine($"  Param: {p.Name} : {p.Type}");

// LocalizationInfo type
var liType = sideloader.GetTypes().FirstOrDefault(t => t.Name == "LocalizationInfo");
Console.WriteLine("\n=== LocalizationInfo fields ===");
if (liType != null)
    foreach (var f in liType.Fields)
        Console.WriteLine($"  {f.Name}: {f.FieldType}");
Console.WriteLine("=== LocalizationInfo properties ===");
if (liType != null)
    foreach (var p in liType.Properties)
        Console.WriteLine($"  {p.Name}: {p.PropertySig?.RetType}");

// LocalizationOption
var loType = sideloader.GetTypes().FirstOrDefault(t => t.Name == "LocalizationOption");
Console.WriteLine("\n=== LocalizationOption ===");
if (loType != null)
{
    foreach (var f in loType.Fields)
        Console.WriteLine($"  field: {f.Name}: {f.FieldType}");
    foreach (var p in loType.Properties)
        Console.WriteLine($"  prop: {p.Name}: {p.PropertySig?.RetType}");
    foreach (var m in loType.Methods.Where(m => !m.IsSpecialName))
        Console.WriteLine($"  method: {m.Name}");
}

// LBoL.Core.Locale enum values
var corePath2 = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.Core.dll";
var core2 = ModuleDefMD.Load(corePath2);
var localeType = core2.GetTypes().FirstOrDefault(t => t.Name == "Locale");
Console.WriteLine("\n=== LBoL.Core.Locale values ===");
if (localeType != null)
    foreach (var f in localeType.Fields.Where(f => f.IsStatic && f.IsLiteral))
        Console.WriteLine($"  {f.Name} = {f.Constant?.Value}");

// How to get current locale
Console.WriteLine("\n=== LBoL.Core types with 'Locale' ===");
foreach (var t in core2.GetTypes().Where(t => t.Name.Contains("Locale") || t.Name.Contains("Language")))
    Console.WriteLine($"  {t.FullName}");

// EnemyUnitTemplate.LoadLocalization
var eut = sideloader.GetTypes().FirstOrDefault(t => t.Name == "EnemyUnitTemplate");
Console.WriteLine("\n=== EnemyUnitTemplate.LoadLocalization ===");
var ll2 = eut?.Methods.FirstOrDefault(x => x.Name == "LoadLocalization");
if (ll2?.Body != null)
    foreach (var instr in ll2.Body.Instructions)
        Console.WriteLine($"  {instr}");
else
    Console.WriteLine("  not found");

var corePath = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.Core.dll";
var core = ModuleDefMD.Load(corePath);

// List all GunConfig entries to find valid IDs
var configPath = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.ConfigData.dll";
var configDll = ModuleDefMD.Load(configPath);

// Find the GunConfig data array via static initializer or _data field
// Actually just print the first ~30 IDs so we know valid ones
// We'll use reflection at runtime instead; just show the struct layout
var gunConfigType = configDll.GetTypes().FirstOrDefault(t => t.Name == "GunConfig");
Console.WriteLine("=== GunConfig fields ===");
if (gunConfigType != null)
    foreach (var f in gunConfigType.Fields.Where(f => !f.IsStatic))
        Console.WriteLine($"  {f.Name}: {f.FieldType}");
Console.WriteLine("\n=== GunConfig properties ===");
if (gunConfigType != null)
    foreach (var p in gunConfigType.Properties)
        Console.WriteLine($"  {p.Name}: {p.PropertySig?.RetType}");



// Check Stage.OnEnter
var stageType = core.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Stage");
Console.WriteLine("\n=== Stage.OnEnter ===");
var onEnter = stageType?.Methods.FirstOrDefault(x => x.Name == "OnEnter");
if (onEnter?.Body != null)
    foreach (var instr in onEnter.Body.Instructions)
        Console.WriteLine($"  {instr}");
else
    Console.WriteLine("  not found");

// Check GameEntity.Initialize
var geType = core.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.GameEntity");
Console.WriteLine("\n=== GameEntity.Initialize ===");
var geInit = geType?.Methods.FirstOrDefault(x => x.Name == "Initialize");
if (geInit?.Body != null)
    foreach (var instr in geInit.Body.Instructions)
        Console.WriteLine($"  {instr}");
else
    Console.WriteLine("  not found");

// Check GameRunController.Initialize
var grcType = core.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.GameRunController");
Console.WriteLine("\n=== GameRunController.Initialize ===");
var grcInit = grcType?.Methods.FirstOrDefault(x => x.Name == "Initialize");
if (grcInit?.Body != null)
    foreach (var instr in grcInit.Body.Instructions)
        Console.WriteLine($"  {instr}");
else
    Console.WriteLine("  not found");

// Find all methods in GRC that reference Stage
Console.WriteLine("\n=== GRC methods that call Stage.Initialize ===");
foreach (var m in grcType?.Methods ?? Enumerable.Empty<dnlib.DotNet.MethodDef>())
{
    if (m.Body == null) continue;
    foreach (var instr in m.Body.Instructions)
    {
        var operand = instr.Operand?.ToString() ?? "";
        if (operand.Contains("Stage") && (operand.Contains("Initialize") || operand.Contains(".ctor") || operand.Contains("Create")))
        {
            Console.WriteLine($"  {m.Name}: {instr}");
        }
    }
}

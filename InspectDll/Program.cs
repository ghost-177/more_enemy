using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Linq;

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

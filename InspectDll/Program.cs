using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Linq;

var sideloaderPath = @"D:\software\steam\steamapps\workshop\content\1140150\3572815053\LBoL-Entity-Sideloader.dll";
var corePath = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.Core.dll";
var sideloader = ModuleDefMD.Load(sideloaderPath);
var core = ModuleDefMD.Load(corePath);

// === UnitModelTemplate.Consume IL ===
Console.WriteLine("=== UnitModelTemplate.Consume IL ===");
var umtType = sideloader.GetTypes().FirstOrDefault(t => t.Name == "UnitModelTemplate");
var consume = umtType?.Methods.FirstOrDefault(m => m.Name == "Consume");
if (consume?.Body != null)
    foreach (var instr in consume.Body.Instructions)
        Console.WriteLine($"  {instr}");
else
    Console.WriteLine("  not found");

// === All StatusEffect types in LBoL.Core.StatusEffects ===
Console.WriteLine("\n=== All SE types in LBoL.Core.StatusEffects ===");
foreach (var t in core.GetTypes())
    if (t.Namespace?.Contains("StatusEffects") == true && !t.IsAbstract && t.BaseType != null)
        Console.WriteLine($"  {t.FullName}  (base: {t.BaseType.Name})");

// === NegativeMove signature ===
Console.WriteLine("\n=== EnemyUnit.NegativeMove signature ===");
var euType = core.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Units.EnemyUnit");
var negMove = euType?.Methods.FirstOrDefault(m => m.Name == "NegativeMove");
if (negMove != null)
{
    Console.WriteLine($"  {negMove.ReturnType} NegativeMove(");
    foreach (var p in negMove.Parameters.Skip(1))
        Console.WriteLine($"    {p.Type} {p.Name}");
    Console.WriteLine("  )");
}

// === PositiveMove signature ===
Console.WriteLine("\n=== EnemyUnit.PositiveMove signature ===");
var posMove = euType?.Methods.FirstOrDefault(m => m.Name == "PositiveMove");
if (posMove != null)
{
    Console.WriteLine($"  {posMove.ReturnType} PositiveMove(");
    foreach (var p in posMove.Parameters.Skip(1))
        Console.WriteLine($"    {p.Type} {p.Name}");
    Console.WriteLine("  )");
}

// === AddCardMove signature (type-based) ===
Console.WriteLine("\n=== EnemyUnit.AddCardMove (type-based) signature ===");
var addCardMoves = euType?.Methods.Where(m => m.Name == "AddCardMove").ToList();
foreach (var m in addCardMoves ?? new())
{
    Console.WriteLine($"  {m.ReturnType} AddCardMove(");
    foreach (var p in m.Parameters.Skip(1))
        Console.WriteLine($"    {p.Type} {p.Name}");
    Console.WriteLine("  )");
}

// === What curse/status cards exist? Check cards with Status/Curse type ===
Console.WriteLine("\n=== Core card types (Status/Curse related) ===");
foreach (var t in core.GetTypes())
    if ((t.Name.Contains("Curse") || t.Name.Contains("Status") || t.Name.Contains("Misfortune") || t.Name.Contains("Decay") || t.Name.Contains("Wound")) && t.BaseType?.Name == "Card")
        Console.WriteLine($"  {t.FullName}");

// === Weak SE fields ===
Console.WriteLine("\n=== Weak SE ===");
var weakType = core.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.StatusEffects.Weak");
if (weakType != null)
{
    Console.WriteLine($"  {weakType.FullName} (base: {weakType.BaseType?.Name})");
}

// === EnemyVulnerable SE ===
Console.WriteLine("\n=== EnemyVulnerable SE ===");
var evType = core.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.StatusEffects.EnemyVulnerable");
if (evType != null) Console.WriteLine($"  {evType.FullName}");
var vType = core.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.StatusEffects.Vulnerable");
if (vType != null) Console.WriteLine($"  {vType.FullName}");

// === All negative SE types (by checking base class) ===
Console.WriteLine("\n=== All SE types with 'base: NegativeSe or StatusEffect' ===");
foreach (var t in core.GetTypes())
{
    if (t.Namespace?.Contains("StatusEffects") == true || t.Namespace?.Contains("StatusEffect") == true)
    {
        var b = t.BaseType?.Name ?? "";
        Console.WriteLine($"  {t.Name}  (base: {b})");
    }
}

// === PerformAction default value ===
Console.WriteLine("\n=== PerformAction type ===");
var paType = core.GetTypes().FirstOrDefault(t => t.Name == "PerformAction");
if (paType != null)
{
    Console.WriteLine($"  IsEnum: {paType.IsEnum}");
    if (paType.IsEnum)
        foreach (var f in paType.Fields.Where(f => f.IsStatic && f.IsLiteral))
            Console.WriteLine($"    {f.Name} = {f.Constant?.Value}");
}

// === CurseCard or WoundCard types in LBoL ===
Console.WriteLine("\n=== All card types that have 'curse', 'wound', 'decay' in name ===");
foreach (var t in core.GetTypes())
    if (t.Name.ToLower().Contains("curse") || t.Name.ToLower().Contains("wound") || t.Name.ToLower().Contains("decay") || t.Name.ToLower().Contains("junk") || t.Name.ToLower().Contains("scrap"))
        Console.WriteLine($"  {t.FullName}");

// Also check entity lib
var entityLibPath = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.EntityLib.dll";
try {
    var elib = ModuleDefMD.Load(entityLibPath);
    Console.WriteLine("\n=== EntityLib curse/wound/decay cards ===");
    foreach (var t in elib.GetTypes())
        if (t.Name.ToLower().Contains("curse") || t.Name.ToLower().Contains("wound") || t.Name.ToLower().Contains("decay") || t.Name.ToLower().Contains("junk"))
            Console.WriteLine($"  {t.FullName}");
} catch (Exception ex) {
    Console.WriteLine($"  EntityLib load failed: {ex.Message}");
}

using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Linq;

// Check the EntityLib for actual formation names used by vanilla enemies
var elPath = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.EntityLib.dll";
var el = ModuleDefMD.Load(elPath);

// Check all EnemyGroupTemplate derived classes
Console.WriteLine("=== All EnemyGroup MakeConfig in EntityLib ===");
foreach (var type in el.Types)
{
    var makeConfig = type.Methods.FirstOrDefault(m => m.Name == "MakeConfig" && m.HasBody);
    if (makeConfig == null) continue;

    // Check if it creates/modifies EnemyGroupConfig
    bool isEnemyGroup = makeConfig.Body.Instructions
        .Any(i => i.Operand?.ToString()?.Contains("EnemyGroupConfig") == true ||
                  i.Operand?.ToString()?.Contains("EnemyType") == true);

    if (isEnemyGroup)
    {
        var instrs = makeConfig.Body.Instructions.ToList();
        var strings = instrs.Where(i => i.OpCode == OpCodes.Ldstr)
            .Select(i => (string)i.Operand).Distinct();
        Console.WriteLine($"  {type.Name}:");
        foreach (var s in strings)
            Console.WriteLine($"    \"{s}\"");
    }
}

// VanillaFormations constants - check bin copy of sideloader that has different version
var binSl = @"D:\riderProject\lbol-sample-adventure\bin\Debug\netstandard2.1\LBoL-Entity-Sideloader.dll";
var sl = ModuleDefMD.Load(binSl);
Console.WriteLine("\n=== All types with 'Formation' or 'Vanilla' in bin sideloader ===");
foreach (var t in sl.Types)
    if (t.FullName.Contains("ormation") || t.FullName.Contains("anilla"))
        Console.WriteLine($"  {t.FullName}");

// Find all string "Single" context in bin sideloader
Console.WriteLine("\n=== DefaultConfig in bin sideloader EnemyGroupTemplate ===");
var egt = sl.Types.FirstOrDefault(t => t.Name == "EnemyGroupTemplate");
if (egt != null)
{
    var dc = egt.Methods.FirstOrDefault(m => m.Name == "DefaultConfig");
    if (dc?.HasBody == true)
    {
        foreach (var inst in dc.Body.Instructions.Where(i => i.Operand != null))
            Console.WriteLine($"  {inst.OpCode} {inst.Operand}");
    }
}

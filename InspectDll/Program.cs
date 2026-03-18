using dnlib.DotNet;
using System;
using System.Linq;

var corePath = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.Core.dll";
var core = ModuleDefMD.Load(corePath);

// Exhibit inheritance chain
Console.WriteLine("=== Exhibit base class chain ===");
var t = core.Types.FirstOrDefault(x => x.Name == "Exhibit");
while (t != null) {
    var gameRunProp = t.Properties.FirstOrDefault(p => p.Name == "GameRun");
    Console.WriteLine($"  {t.FullName} - GameRun: {(gameRunProp != null ? gameRunProp.PropertySig?.RetType?.ToString() : "no")}");
    t = t.BaseType != null ? core.Types.FirstOrDefault(x => x.FullName == t.BaseType.FullName) : null;
}

// BlockShieldType enum
Console.WriteLine("\n=== BlockShieldType enum ===");
var bst = core.Types.FirstOrDefault(x => x.Name == "BlockShieldType");
if (bst != null)
    foreach (var f in bst.Fields.Where(f => f.IsLiteral))
        Console.WriteLine($"  {f.Name} = {f.Constant?.Value}");

// BattleController - accessible from Exhibit.Battle: check CardUsed and other events
Console.WriteLine("\n=== BattleController events ===");
var bc = core.Types.FirstOrDefault(x => x.Name == "BattleController");
if (bc != null)
    foreach (var p in bc.Properties.Where(p => p.PropertySig?.RetType?.TypeName?.Contains("Event") == true))
        Console.WriteLine($"  {p.PropertySig?.RetType} {p.Name}");

// CardUsingEventArgs
Console.WriteLine("\n=== CardUsingEventArgs fields ===");
var cuea = core.Types.FirstOrDefault(x => x.Name == "CardUsingEventArgs");
if (cuea != null)
    foreach (var p in cuea.Properties)
        Console.WriteLine($"  {p.PropertySig?.RetType} {p.Name}");

// UnitEventArgs - for turn events
Console.WriteLine("\n=== UnitEventArgs fields ===");
var uea = core.Types.FirstOrDefault(x => x.Name == "UnitEventArgs");
if (uea != null)
    foreach (var p in uea.Properties)
        Console.WriteLine($"  {p.PropertySig?.RetType} {p.Name}");

// DieEventArgs (enemy death)
Console.WriteLine("\n=== Die / Kill event args ===");
foreach (var type in core.Types.Where(x => x.Name.Contains("Die") || x.Name.Contains("Kill") || x.Name.Contains("Death")))
    Console.WriteLine($"  {type.FullName}");

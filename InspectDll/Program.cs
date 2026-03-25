using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Linq;

var sideloaderPath = @"D:\software\steam\steamapps\workshop\content\1140150\3572815053\LBoL-Entity-Sideloader.dll";
var corePath = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.Core.dll";
var sideloader = ModuleDefMD.Load(sideloaderPath);
var core = ModuleDefMD.Load(corePath);

// === EnemyUnit OnEnterBattle signature ===
Console.WriteLine("=== EnemyUnit OnEnterBattle and related methods ===");
var euType2 = core.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Units.EnemyUnit");
if (euType2 != null) {
    foreach (var m in euType2.Methods.Where(m => m.Name.Contains("Enter") || m.Name.Contains("Battle") || m.Name.Contains("React") || m.Name.Contains("Apply")))
        Console.WriteLine($"  [{(m.IsVirtual?"virtual":"")}{(m.IsAbstract?"abstract":"")}] {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
}

// Check if EntityLib EnemyUnit subclasses apply SEs in OnEnterBattle using React
Console.WriteLine("\n=== EntityLib EnemyUnit.OnEnterBattle with React/ApplyStatusEffect ===");
try {
    var elib2 = ModuleDefMD.Load(@"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.EntityLib.dll");
    var enemyUnitFqn = "LBoL.Core.Units.EnemyUnit";
    int cnt2 = 0;
    foreach (var t in elib2.GetTypes()) {
        // Only EnemyUnit subclasses
        if (t.BaseType?.FullName != enemyUnitFqn && t.BaseType?.FullName != null) {
            // Walk the chain
            var bt = t.BaseType?.ResolveTypeDef();
            bool isEnemy = false;
            for (int depth = 0; depth < 5 && bt != null; depth++) {
                if (bt.FullName == enemyUnitFqn) { isEnemy = true; break; }
                bt = bt.BaseType?.ResolveTypeDef();
            }
            if (!isEnemy) continue;
        } else if (t.BaseType?.FullName != enemyUnitFqn) continue;

        foreach (var m in t.Methods.Where(m => m.Name == "OnEnterBattle")) {
            if (m.Body != null && m.Body.Instructions.Any(i =>
                i.Operand?.ToString()?.Contains("ApplyStatusEffect") == true ||
                i.Operand?.ToString()?.Contains("React") == true)) {
                Console.WriteLine($"  {t.Name}.{m.Name}:");
                foreach (var instr in m.Body.Instructions.Take(50))
                    Console.WriteLine($"    {instr}");
                if (++cnt2 >= 2) goto doneSE;
            }
        }
    }
    doneSE: ;
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Find BattleAction for gaining shield/block
Console.WriteLine("\n=== BattleActions for gaining shield/block ===");
try {
    var coreForBlock = ModuleDefMD.Load(corePath);
    foreach (var t in coreForBlock.GetTypes())
    {
        if (t.Name.Contains("Block") || t.Name.Contains("Shield") || t.Name.Contains("Defend") || t.Name.Contains("Guard"))
        {
            if (t.BaseType?.Name?.Contains("BattleAction") == true || t.BaseType?.Name?.Contains("Action") == true)
            {
                Console.WriteLine($"  {t.FullName} : {t.BaseType?.FullName}");
                foreach (var c in t.Methods.Where(m => m.Name == ".ctor"))
                    Console.WriteLine($"    ctor({string.Join(", ", c.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
            }
        }
    }
    var elibForBlock = ModuleDefMD.Load(@"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.EntityLib.dll");
    foreach (var t in elibForBlock.GetTypes())
    {
        if (t.Name.Contains("Block") || t.Name.Contains("Shield") || t.Name.Contains("Defend") || t.Name.Contains("Guard"))
        {
            if (t.BaseType?.Name?.Contains("BattleAction") == true || t.BaseType?.Name?.Contains("Action") == true)
            {
                Console.WriteLine($"  [elib]{t.FullName} : {t.BaseType?.FullName}");
                foreach (var c in t.Methods.Where(m => m.Name == ".ctor"))
                    Console.WriteLine($"    ctor({string.Join(", ", c.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
            }
        }
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Find GainBlockAction or similar in BattleActions namespace
Console.WriteLine("\n=== All BattleActions (Gain/Add/Apply prefix) ===");
try {
    var coreForActions = ModuleDefMD.Load(corePath);
    foreach (var t in coreForActions.GetTypes())
    {
        if (t.Namespace?.Contains("BattleAction") == true && (t.Name.StartsWith("Gain") || t.Name.StartsWith("Add") || t.Name.StartsWith("Give")))
        {
            Console.WriteLine($"  {t.Name}");
            foreach (var c in t.Methods.Where(m => m.Name == ".ctor"))
                Console.WriteLine($"    ctor({string.Join(", ", c.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
        }
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Check if Shield is a StatusEffect
Console.WriteLine("\n=== Shield StatusEffect in EntityLib ===");
try {
    var elibForSe = ModuleDefMD.Load(@"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.EntityLib.dll");
    var shieldSe = elibForSe.GetTypes().FirstOrDefault(t => t.Name == "Shield" || t.Name == "ShieldSe");
    if (shieldSe != null) Console.WriteLine($"  Found: {shieldSe.FullName} : {shieldSe.BaseType?.FullName}");
    else Console.WriteLine("  Shield SE not found");
    // Try to find Unit.Shield property
    var coreForShield = ModuleDefMD.Load(corePath);
    var unitTypeForShield = coreForShield.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Units.Unit");
    if (unitTypeForShield != null)
    {
        foreach (var p in unitTypeForShield.Properties.Where(p => p.Name.Contains("Shield") || p.Name.Contains("Block") || p.Name.Contains("Defend")))
            Console.WriteLine($"  Unit prop: {p.PropertySig?.RetType} {p.Name}");
        foreach (var m in unitTypeForShield.Methods.Where(m => m.Name.Contains("Shield") || m.Name.Contains("Block") || m.Name.Contains("Defend")))
            Console.WriteLine($"  Unit method: {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Find BlockShieldType enum values
Console.WriteLine("\n=== BlockShieldType enum ===");
try {
    var coreForBst = ModuleDefMD.Load(corePath);
    var bstType = coreForBst.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.BlockShieldType");
    if (bstType != null)
        foreach (var f in bstType.Fields.Where(f => f.IsLiteral))
            Console.WriteLine($"  {f.Name} = {f.Constant?.Value}");
    // Also find AddCardsType
    var actType = coreForBst.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.AddCardsType");
    if (actType != null) {
        Console.WriteLine("  [AddCardsType]");
        foreach (var f in actType.Fields.Where(f => f.IsLiteral))
            Console.WriteLine($"    {f.Name} = {f.Constant?.Value}");
    }
    // Find Unit damage/being attacked events
    var unitType2 = coreForBst.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Units.Unit");
    Console.WriteLine("  [Unit Damage events]");
    if (unitType2 != null)
        foreach (var p in unitType2.Properties.Where(p => p.Name.Contains("Damag") || p.Name.Contains("Hit") || p.Name.Contains("Attack") || p.Name.Contains("Hurt")))
            Console.WriteLine($"    {p.PropertySig?.RetType} {p.Name}");
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Find how to create card instances (Library.CreateCard or GameRunController.CreateCard)
Console.WriteLine("\n=== Library/CreateCard methods ===");
try {
    var coreForCard = ModuleDefMD.Load(corePath);
    var libType = coreForCard.GetTypes().FirstOrDefault(t => t.Name == "Library" || t.Name == "CardLibrary");
    if (libType != null)
    {
        Console.WriteLine($"  Found: {libType.FullName}");
        foreach (var m in libType.Methods.Where(m => m.Name.Contains("Create") || m.Name.Contains("Make")))
            Console.WriteLine($"  {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(m.IsStatic?0:1).Select(p => p.Type + " " + p.Name))})");
    }
    // Check GameRunController for card creation
    var grcType = coreForCard.GetTypes().FirstOrDefault(t => t.Name == "GameRunController");
    if (grcType != null)
    {
        Console.WriteLine("  [GameRunController CreateCard methods]");
        foreach (var m in grcType.Methods.Where(m => m.Name.Contains("Create") || m.Name.Contains("MakeCard")))
            Console.WriteLine($"  {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(m.IsStatic?0:1).Select(p => p.Type + " " + p.Name))})");
    }
    // Check Battle/BattleController for card creation
    var bcType2 = coreForCard.GetTypes().FirstOrDefault(t => t.Name == "BattleController");
    if (bcType2 != null)
    {
        Console.WriteLine("  [BattleController CreateCard methods]");
        foreach (var m in bcType2.Methods.Where(m => m.Name.Contains("Create") || m.Name.Contains("MakeCard")))
            Console.WriteLine($"  {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(m.IsStatic?0:1).Select(p => p.Type + " " + p.Name))})");
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Find all SE types in EntityLib with "evade", "dodge", "elude", "swift"
Console.WriteLine("\n=== All enemy-relevant SE types (Evasion search) ===");
try {
    var elibForSe2 = ModuleDefMD.Load(@"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.EntityLib.dll");
    // All SEs in StatusEffects.Enemy namespace
    foreach (var t in elibForSe2.GetTypes())
    {
        if (t.Namespace?.Contains("StatusEffect") == true || t.Namespace?.Contains("Enemy") == true)
            if (t.BaseType?.FullName?.Contains("StatusEffect") == true)
                Console.WriteLine($"  {t.Name}");
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Find all SEs in LBoL.Core.StatusEffects namespace
Console.WriteLine("\n=== LBoL.Core StatusEffects ===");
try {
    var coreForSe = ModuleDefMD.Load(corePath);
    foreach (var t in coreForSe.GetTypes())
        if (t.Namespace?.Contains("StatusEffect") == true && t.BaseType?.Name?.Contains("StatusEffect") == true)
            Console.WriteLine($"  {t.Name}");
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Find Library on GameRunController (full search)
Console.WriteLine("\n=== GRC full property list (Library search) ===");
try {
    var coreForGrc = ModuleDefMD.Load(corePath);
    var grcType3 = coreForGrc.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.GameRunController");
    if (grcType3 != null)
    {
        foreach (var p in grcType3.Properties)
            if (p.PropertySig?.RetType?.FullName?.Contains("Library") == true || p.Name.Contains("Library") || p.Name.Contains("Lib"))
                Console.WriteLine($"  {p.PropertySig?.RetType} {p.Name}");
        // Also check fields
        foreach (var f in grcType3.Fields)
            if (f.FieldType?.FullName?.Contains("Library") == true || f.Name.Contains("Library") || f.Name.Contains("library"))
                Console.WriteLine($"  field: {f.FieldType} {f.Name}");
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Find Evasion/Dodge StatusEffect in EntityLib
Console.WriteLine("\n=== Evasion/Dodge SE type ===");
try {
    var elibForEva = ModuleDefMD.Load(@"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.EntityLib.dll");
    foreach (var t in elibForEva.GetTypes())
    {
        if (t.Name.Contains("Evad") || t.Name.Contains("Dodge") || t.Name.Contains("Miss") || t.Name.Contains("闪"))
            Console.WriteLine($"  {t.FullName} : {t.BaseType?.Name}");
    }
    // Also look in LBoL.Core
    var coreForEva = ModuleDefMD.Load(corePath);
    foreach (var t in coreForEva.GetTypes())
    {
        if (t.Name.Contains("Evad") || t.Name.Contains("Dodge") || t.Name.Contains("Miss"))
            Console.WriteLine($"  [core]{t.FullName} : {t.BaseType?.Name}");
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// How to access Library from StatusEffect / BattleController
Console.WriteLine("\n=== BattleController GameRun/Library property ===");
try {
    var coreForLib = ModuleDefMD.Load(corePath);
    var bcType3 = coreForLib.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Battle.BattleController");
    if (bcType3 != null)
    {
        foreach (var p in bcType3.Properties.Where(p => p.Name.Contains("Run") || p.Name.Contains("Library") || p.Name.Contains("Game")))
            Console.WriteLine($"  BC prop: {p.PropertySig?.RetType} {p.Name}");
    }
    // BattleController fields
    var grcType2 = coreForLib.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.GameRunController");
    if (grcType2 != null)
    {
        foreach (var p in grcType2.Properties.Where(p => p.Name.Contains("Lib") || p.Name.Contains("lib")))
            Console.WriteLine($"  GRC prop: {p.PropertySig?.RetType} {p.Name}");
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Check EnemyUnit.AddCardMove(Type) IL to see how cards are created
Console.WriteLine("\n=== EnemyUnit.AddCardMove(Type) IL ===");
try {
    var coreForAcm = ModuleDefMD.Load(corePath);
    var euForAcm = coreForAcm.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Units.EnemyUnit");
    if (euForAcm != null) {
        // Find the AddCardMove overload that takes Type
        var typeBasedAddCard = euForAcm.Methods.FirstOrDefault(m => m.Name == "AddCardMove" &&
            m.Parameters.Any(p => p.Type?.FullName == "System.Type"));
        if (typeBasedAddCard?.Body != null) {
            foreach (var i in typeBasedAddCard.Body.Instructions)
                Console.WriteLine($"  {i}");
        }
    }
    // Also check GRC base types for Library
    var grcType4 = coreForAcm.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.GameRunController");
    if (grcType4 != null) {
        Console.WriteLine("  [GRC base types]");
        var bt2 = grcType4.BaseType?.ResolveTypeDef();
        for (int depth = 0; depth < 3 && bt2 != null; depth++) {
            Console.WriteLine($"    base: {bt2.FullName}");
            foreach (var p in bt2.Properties.Where(p => p.Name.Contains("Lib") || p.Name.Contains("lib")))
                Console.WriteLine($"      prop: {p.PropertySig?.RetType} {p.Name}");
            bt2 = bt2.BaseType?.ResolveTypeDef();
        }
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }
try {
    var coreForAcm = ModuleDefMD.Load(corePath);
    var euForAcm = coreForAcm.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Units.EnemyUnit");
    if (euForAcm != null) {
        var addCardMethods = euForAcm.Methods.Where(m => m.Name == "AddCardMove").ToList();
        foreach (var m in addCardMethods) {
            Console.WriteLine($"  {m.ReturnType} AddCardMove({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
        }
        // Find InternalAddCards or any add card action creation
        var createCardMethod = euForAcm.Methods.FirstOrDefault(m => m.Name.Contains("Card") && m.Body != null);
        if (createCardMethod != null) {
            Console.WriteLine($"  First card-related method: {createCardMethod.Name}");
            foreach (var i in createCardMethod.Body.Instructions.Take(20))
                Console.WriteLine($"    {i}");
        }
    }
    // Also search for Library instance or singleton
    var libraryType = coreForAcm.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Library");
    if (libraryType != null) {
        Console.WriteLine("  [Library]:");
        foreach (var f in libraryType.Fields.Where(f => f.IsStatic))
            Console.WriteLine($"    static {f.FieldType} {f.Name}");
        foreach (var p in libraryType.Properties.Where(p => p.GetMethod?.IsStatic == true))
            Console.WriteLine($"    static prop: {p.PropertySig?.RetType} {p.Name}");
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Check what Graze SE does (evasion mechanic?)
Console.WriteLine("\n=== Graze SE config ===");
try {
    var elibForGraze = ModuleDefMD.Load(@"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.EntityLib.dll");
    var grazeDef = elibForGraze.GetTypes().FirstOrDefault(t => t.Name == "GrazeDef" || t.Name == "GrazeD");
    if (grazeDef == null) {
        var coreForGraze = ModuleDefMD.Load(corePath);
        grazeDef = coreForGraze.GetTypes().FirstOrDefault(t => t.Name == "GrazeDef");
    }
    if (grazeDef != null) {
        Console.WriteLine($"  Found: {grazeDef.FullName}");
        var mc = grazeDef.Methods.FirstOrDefault(m => m.Name == "MakeConfig");
        if (mc?.Body != null)
            foreach (var i in mc.Body.Instructions) Console.WriteLine($"    {i}");
    }
    // Also check MirrorImage
    var coreForMirror = ModuleDefMD.Load(corePath);
    var mirrorType = coreForMirror.GetTypes().FirstOrDefault(t => t.Name == "MirrorImage");
    if (mirrorType != null) Console.WriteLine($"  MirrorImage found: {mirrorType.FullName}");
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Also check EnemyUnit base class methods including React
Console.WriteLine("\n=== EnemyUnit React method signature ===");
try {
    var elibCore = ModuleDefMD.Load(corePath);
    var euType3 = elibCore.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Units.EnemyUnit");
    if (euType3 != null) {
        foreach (var m in euType3.Methods.Where(m => m.Name.Contains("React") || m.Name.Contains("Execute")))
            Console.WriteLine($"  [{(m.IsVirtual?"virtual":"")}{(m.IsAbstract?"abstract":"")}] {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
    }
    // Also check GameEntity/Unit for React
    var gameEntityType = elibCore.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.GameEntity");
    if (gameEntityType != null) {
        Console.WriteLine("  [GameEntity React methods:]");
        foreach (var m in gameEntityType.Methods.Where(m => m.Name.Contains("React") || m.Name == "Execute"))
            Console.WriteLine($"  [{(m.IsVirtual?"virtual":"")}{(m.IsAbstract?"abstract":"")}] {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// === Unit TurnStarted/TurnEnded events ===
Console.WriteLine("=== Unit events (Turn related) ===");
var unitType = core.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Units.Unit");
if (unitType != null) {
    foreach (var p in unitType.Properties)
        if (p.Name.Contains("Turn") || p.Name.Contains("Draw") || p.Name.Contains("Round"))
            Console.WriteLine($"  prop: {p.PropertySig?.RetType} {p.Name}");
    foreach (var f in unitType.Fields)
        if (f.Name.Contains("Turn") || f.Name.Contains("Draw") || f.Name.Contains("Round"))
            Console.WriteLine($"  field: {f.FieldType} {f.Name}");
}

// === BattleController Player property ===
Console.WriteLine("\n=== BattleController Player events ===");
var bcType = core.GetTypes().FirstOrDefault(t => t.FullName == "LBoL.Core.Battle.BattleController");
if (bcType != null) {
    foreach (var p in bcType.Properties)
        if (p.Name.Contains("Player") || p.Name.Contains("Turn") || p.Name.Contains("Round"))
            Console.WriteLine($"  prop: {p.PropertySig?.RetType} {p.Name}");
}

// === ApplyStatusEffectAction constructors ===
Console.WriteLine("\n=== ApplyStatusEffectAction ctor ===");
var aseaType = core.GetTypes().FirstOrDefault(t => t.Name == "ApplyStatusEffectAction");
if (aseaType != null) {
    foreach (var c in aseaType.Methods.Where(m => m.Name == ".ctor"))
        Console.WriteLine($"  ctor({string.Join(", ", c.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
}

// === Weak SE config (HasDuration, DurationDecreaseTiming) ===
Console.WriteLine("=== Weak StatusEffectConfig ===");
var entityLibPath3 = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.EntityLib.dll";
try {
    var elib = ModuleDefMD.Load(entityLibPath3);
    // Find Weak class and its MakeConfig method
    var weakClass = elib.GetTypes().FirstOrDefault(t => t.Name == "Weak");
    if (weakClass != null) {
        Console.WriteLine($"  Found: {weakClass.FullName}");
        var makeCfg = weakClass.Methods.FirstOrDefault(m => m.Name == "MakeConfig");
        if (makeCfg?.Body != null)
            foreach (var instr in makeCfg.Body.Instructions)
                Console.WriteLine($"    {instr}");
    }
    // Also check DurationDecreaseTiming enum
    Console.WriteLine("\n=== DurationDecreaseTiming enum ===");
    var ddt = core.GetTypes().FirstOrDefault(t => t.Name == "DurationDecreaseTiming");
    if (ddt?.IsEnum == true)
        foreach (var f in ddt.Fields.Where(f => f.IsStatic && f.IsLiteral))
            Console.WriteLine($"  {f.Name} = {f.Constant?.Value}");
    // Find all enemies that use NegativeMove with Vulnerable in EntityLib
    Console.WriteLine("\n=== EntityLib enemy NegativeMove with Vulnerable ===");
    int cnt = 0;
    foreach (var t in elib.GetTypes()) {
        foreach (var m in t.Methods) {
            if (m.Body != null && m.Body.Instructions.Any(i => i.Operand?.ToString()?.Contains("Vulnerable") == true)
                && m.Body.Instructions.Any(i => i.Operand?.ToString()?.Contains("NegativeMove") == true)) {
                Console.WriteLine($"  {t.Name}.{m.Name}:");
                // just show the NegativeMove call area
                var instrs = m.Body.Instructions.ToList();
                for (int i2 = 0; i2 < instrs.Count; i2++) {
                    if (instrs[i2].Operand?.ToString()?.Contains("NegativeMove") == true) {
                        for (int j = Math.Max(0, i2-15); j <= i2; j++)
                            Console.WriteLine($"    {instrs[j]}");
                        break;
                    }
                }
                if (++cnt >= 2) goto doneSearch2;
            }
        }
    }
    doneSearch2: ;

    // Also show NegativeMove with Weak (the Cirno one) - just the relevant part
    Console.WriteLine("\n=== EntityLib Cirno NegativeMove call area ===");
    foreach (var t in elib.GetTypes().Where(t => t.Name == "Cirno")) {
        foreach (var m in t.Methods) {
            if (m.Body != null && m.Body.Instructions.Any(i => i.Operand?.ToString()?.Contains("NegativeMove") == true)) {
                var instrs = m.Body.Instructions.ToList();
                for (int i2 = 0; i2 < instrs.Count; i2++) {
                    if (instrs[i2].Operand?.ToString()?.Contains("NegativeMove") == true) {
                        for (int j = Math.Max(0, i2-20); j <= i2; j++)
                            Console.WriteLine($"  {instrs[j]}");
                        break;
                    }
                }
            }
        }
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// === ResourcesHelper ALL methods ===
Console.WriteLine("=== ResourcesHelper ALL methods ===");
// === ALL ResourcesHelper methods including private ===
Console.WriteLine("=== ResourcesHelper ALL methods including private ===");
try {
    var presPath = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.Presentation.dll";
    var pres = ModuleDefMD.Load(presPath);
    var rhPres = pres.GetTypes().FirstOrDefault(t => t.Name == "ResourcesHelper");
    if (rhPres != null) {
        foreach (var m in rhPres.Methods)
            Console.WriteLine($"  [{(m.IsPublic?"pub":m.IsPrivate?"priv":"intl")}]{(m.IsStatic?" static":"")} {m.ReturnType.FullName} {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
    }
} catch { }

// === ResourceLoader in sideloader ===
Console.WriteLine("=== ResourceLoader in sideloader ===");
var rlType = sideloader.GetTypes().FirstOrDefault(t => t.Name == "ResourceLoader");
if (rlType != null) {
    foreach (var m in rlType.Methods)
        Console.WriteLine($"  {(m.IsStatic?"static":"")} {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
}

// Check if there's some extension that wraps ResourcesHelper with a string param
Console.WriteLine("\n=== Any sideloader method with 'LoadSpine' or 'LoadSprite' ===");
foreach (var t in sideloader.GetTypes()) {
    foreach (var m in t.Methods) {
        if (m.Name.Contains("LoadSpin") || m.Name.Contains("LoadSprite") || m.Name.Contains("LoadSimple") || m.Name.Contains("LoadPortrait")) {
            Console.WriteLine($"  {t.Name}.{m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
        }
    }
}

// === ResourcesHelper FULL in Presentation (correct path) ===
Console.WriteLine("=== ResourcesHelper FULL in Presentation (correct path) ===");
try {
    // Try the path from csproj GameFolder
    string[] presentationPaths = {
        @"D:\Program Files\Steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.Presentation.dll",
        @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.Presentation.dll",
    };
    ModuleDefMD pres = null;
    foreach (var pp in presentationPaths) {
        try { pres = ModuleDefMD.Load(pp); Console.WriteLine($"  Loaded from: {pp}"); break; } catch { }
    }
    if (pres == null) { Console.WriteLine("  Could not load Presentation DLL"); }
    else {
        var rhPres = pres.GetTypes().FirstOrDefault(t => t.Name == "ResourcesHelper");
        if (rhPres != null) {
            Console.WriteLine($"  IsAbstract: {rhPres.IsAbstract} IsSealed: {rhPres.IsSealed}");
            foreach (var m in rhPres.Methods)
                Console.WriteLine($"  {(m.IsStatic?"static":"")} {m.ReturnType.FullName} {m.Name}({string.Join(", ", m.Parameters.Select(p => p.Type + " " + p.Name))})");
        }
    }
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// === ResourcesHelper in Presentation ===
Console.WriteLine("=== ResourcesHelper in Presentation ===");
try {
    var presPath = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.Presentation.dll";
    var pres = ModuleDefMD.Load(presPath);
    var rhPres = pres.GetTypes().FirstOrDefault(t => t.Name == "ResourcesHelper");
    if (rhPres != null) {
        foreach (var m in rhPres.Methods.Where(m => !m.IsConstructor))
            Console.WriteLine($"  {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
    } else Console.WriteLine("  not found in Presentation");

    // Also check UnitModelConfig in presentation
    var umcPres = pres.GetTypes().FirstOrDefault(t => t.Name == "UnitModelConfig");
    if (umcPres != null) Console.WriteLine($"  UnitModelConfig found in Presentation!");
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// Check the csproj references for where ResourcesHelper might be
var rhType2 = sideloader.GetTypes().FirstOrDefault(t => t.Name.Contains("ResourcesHelper"));
if (rhType2 != null) {
    Console.WriteLine($"  IsStatic: {rhType2.IsAbstract && rhType2.IsSealed}");
    foreach (var m in rhType2.Methods)
        Console.WriteLine($"  {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))}) static={m.IsStatic}");
} else Console.WriteLine("  NOT FOUND");

// === Check if ResourcesHelper is in another assembly ===
var entityLibPath2 = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.EntityLib.dll";
try {
    var elib = ModuleDefMD.Load(entityLibPath2);
    Console.WriteLine("\n=== EntityLib ResourcesHelper ===");
    var elrhType = elib.GetTypes().FirstOrDefault(t => t.Name == "ResourcesHelper");
    if (elrhType != null)
        foreach (var m in elrhType.Methods)
            Console.WriteLine($"  {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
} catch { }

// === UnitModelConfig vanilla entries ===
Console.WriteLine("=== UnitModelConfig fields ===");
var umcType2 = core.GetTypes().FirstOrDefault(t => t.Name == "UnitModelConfig");
if (umcType2 != null) {
    Console.WriteLine($"  Fields: {string.Join(", ", umcType2.Fields.Select(f => f.FieldType + " " + f.Name))}");
    var ctors = umcType2.Methods.Where(m => m.Name == ".ctor").ToList();
    foreach (var c in ctors)
        Console.WriteLine($"  ctor: ({string.Join(", ", c.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
}

// === UnitView.ModelType enum ===
Console.WriteLine("\n=== UnitView.ModelType ===");
var presentationPath = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed\LBoL.Presentation.dll";
try {
    var presentation = ModuleDefMD.Load(presentationPath);
    var uvType = presentation.GetTypes().FirstOrDefault(t => t.Name == "UnitView");
    var mtType = uvType?.NestedTypes.FirstOrDefault(t => t.Name == "ModelType");
    if (mtType?.IsEnum == true)
        foreach (var f in mtType.Fields.Where(f => f.IsStatic && f.IsLiteral))
            Console.WriteLine($"  {f.Name} = {f.Constant?.Value}");
} catch (Exception ex) { Console.WriteLine($"  error: {ex.Message}"); }

// === ResourcesHelper methods ===
Console.WriteLine("\n=== ResourcesHelper methods ===");
var rhType = sideloader.GetTypes().FirstOrDefault(t => t.Name == "ResourcesHelper");
if (rhType != null)
    foreach (var m in rhType.Methods.Where(m => !m.IsConstructor))
        Console.WriteLine($"  {m.ReturnType} {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");

// === EntityManager.RegisterSelf IL ===
Console.WriteLine("=== EntityManager methods ===");
var emType = sideloader.GetTypes().FirstOrDefault(t => t.Name == "EntityManager");
if (emType != null)
    foreach (var m in emType.Methods.Where(m => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_")))
        Console.WriteLine($"  {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");

// === UnitModelTemplate - what types does it register? ===
Console.WriteLine("\n=== UnitModelTemplate.EntityType IL ===");
var umt3 = sideloader.GetTypes().FirstOrDefault(t => t.Name == "UnitModelTemplate");
var etMethod = umt3?.Methods.FirstOrDefault(m => m.Name == "EntityType");
if (etMethod?.Body != null)
    foreach (var instr in etMethod.Body.Instructions)
        Console.WriteLine($"  {instr}");

// === All sideloader types that may be auto-created ===
Console.WriteLine("\n=== Sideloader types mentioning 'Model' ===");
foreach (var t in sideloader.GetTypes())
    if (t.Name.Contains("Model"))
        Console.WriteLine($"  {t.FullName} base:{t.BaseType?.Name}");

// === EnemyUnitTemplate.Consume IL ===
Console.WriteLine("=== EnemyUnitTemplate.Consume IL ===");
var eutType2 = sideloader.GetTypes().FirstOrDefault(t => t.Name == "EnemyUnitTemplate");
var eutConsume = eutType2?.Methods.FirstOrDefault(m => m.Name == "Consume");
if (eutConsume?.Body != null)
    foreach (var instr in eutConsume.Body.Instructions)
        Console.WriteLine($"  {instr}");
else Console.WriteLine("  not found");

// === EnemyUnitTemplate.DefaultConfig IL ===
Console.WriteLine("\n=== EnemyUnitTemplate.DefaultConfig IL ===");
var eutDefault = eutType2?.Methods.FirstOrDefault(m => m.Name == "DefaultConfig");
if (eutDefault?.Body != null)
    foreach (var instr in eutDefault.Body.Instructions)
        Console.WriteLine($"  {instr}");

// === Inheritance hierarchy ===
Console.WriteLine("=== EnemyUnitTemplate base ===");
var eutBase = sideloader.GetTypes().FirstOrDefault(t => t.Name == "EnemyUnitTemplate");
if (eutBase != null) {
    var cur = eutBase;
    while (cur != null) {
        Console.WriteLine($"  {cur.FullName}");
        cur = cur.BaseType?.ResolveTypeDef();
    }
}

// === UnitModelTemplate.LoadModelOptions ===
Console.WriteLine("\n=== UnitModelTemplate.LoadModelOptions IL ===");
var umtType3 = sideloader.GetTypes().FirstOrDefault(t => t.Name == "UnitModelTemplate");
var lmo = umtType3?.Methods.FirstOrDefault(m => m.Name == "LoadModelOptions");
if (lmo?.Body != null)
    foreach (var instr in lmo.Body.Instructions.Take(30))
        Console.WriteLine($"  {instr}");

// === UnitModelTemplate.CheckModelOptions IL ===
Console.WriteLine("\n=== UnitModelTemplate.CheckModelOptions IL ===");
var cmo = umtType3?.Methods.FirstOrDefault(m => m.Name == "CheckModelOptions");
if (cmo?.Body != null)
    foreach (var instr in cmo.Body.Instructions)
        Console.WriteLine($"  {instr}");

// === EnemyUnitTemplate methods ===
Console.WriteLine("=== EnemyUnitTemplate methods ===");
var eutType = sideloader.GetTypes().FirstOrDefault(t => t.Name == "EnemyUnitTemplate");
if (eutType != null)
    foreach (var m in eutType.Methods)
        Console.WriteLine($"  {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");

// === All methods of base EntityDefinition ===
Console.WriteLine("\n=== EntityDefinition methods ===");
var edType = sideloader.GetTypes().FirstOrDefault(t => t.Name == "EntityDefinition");
if (edType != null)
    foreach (var m in edType.Methods.Where(m => m.Name.Contains("Model") || m.Name.Contains("Sprite") || m.Name.Contains("Icon")))
        Console.WriteLine($"  {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");

// === ModelOption fields/properties ===
Console.WriteLine("\n=== ModelOption members ===");
var moType = sideloader.GetTypes().FirstOrDefault(t => t.Name == "ModelOption");
if (moType != null) {
    Console.WriteLine($"  Fields:");
    foreach (var f in moType.Fields) Console.WriteLine($"    {f.FieldType} {f.Name}");
    Console.WriteLine($"  Properties:");
    foreach (var p in moType.Properties) Console.WriteLine($"    {p.PropertySig?.RetType} {p.Name}");
    Console.WriteLine($"  Methods:");
    foreach (var m in moType.Methods.Where(m => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_")))
        Console.WriteLine($"    {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
}

// === UnitModelTemplate methods ===
Console.WriteLine("\n=== UnitModelTemplate methods ===");
var umtType2 = sideloader.GetTypes().FirstOrDefault(t => t.Name == "UnitModelTemplate");
if (umtType2 != null) {
    foreach (var m in umtType2.Methods) Console.WriteLine($"  {m.Name}({string.Join(", ", m.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))})");
}

// === UnitModelConfig (core) ===
Console.WriteLine("\n=== UnitModelConfig in core ===");
var umcType = core.GetTypes().FirstOrDefault(t => t.Name == "UnitModelConfig");
if (umcType != null) {
    Console.WriteLine($"  Fields:");
    foreach (var f in umcType.Fields) Console.WriteLine($"    {f.FieldType} {f.Name}");
    foreach (var p in umcType.Properties) Console.WriteLine($"    prop: {p.PropertySig?.RetType} {p.Name}");
    // Show constructor params
    var ctor = umcType.Methods.FirstOrDefault(m => m.Name == ".ctor");
    if (ctor != null) Console.WriteLine($"  ctor params: {string.Join(", ", ctor.Parameters.Skip(1).Select(p => p.Type + " " + p.Name))}");
}

// === ModelOption / UnitModelConfig ===
Console.WriteLine("\n=== ModelOption types ===");
foreach (var t in sideloader.GetTypes())
    if (t.Name.Contains("Model") || t.Name.Contains("Option"))
        Console.WriteLine($"  {t.FullName}");
foreach (var t in core.GetTypes())
    if (t.Name.Contains("UnitModelConfig") || t.Name.Contains("ModelOption"))
        Console.WriteLine($"  core: {t.FullName}");

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

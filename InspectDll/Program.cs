using dnlib.DotNet;
using System;
using System.Linq;
using System.IO;

var managedPath = @"D:\software\steam\steamapps\common\LBoL\LBoL_Data\Managed";

// Search all DLLs for ResourcesHelper
foreach (var dll in Directory.GetFiles(managedPath, "*.dll"))
{
    try
    {
        var mod = ModuleDefMD.Load(dll);
        var rh = mod.Types.FirstOrDefault(t => t.Name == "ResourcesHelper");
        if (rh != null)
        {
            Console.WriteLine($"Found ResourcesHelper in {Path.GetFileName(dll)}!");
            Console.WriteLine($"  Namespace: {rh.Namespace}");
            foreach (var m in rh.Methods.Where(m => m.IsStatic && !m.IsConstructor))
                Console.WriteLine($"  {m.ReturnType} {m.Name}(...)");
            break;
        }
    }
    catch { }
}

// Also search BepInEx plugins
var bepinexPath = @"D:\software\steam\steamapps\common\LBoL\BepInEx\plugins";
foreach (var dll in Directory.GetFiles(bepinexPath, "*.dll", SearchOption.AllDirectories))
{
    try
    {
        var mod = ModuleDefMD.Load(dll);
        var rh = mod.Types.FirstOrDefault(t => t.Name == "ResourcesHelper");
        if (rh != null)
        {
            Console.WriteLine($"Found ResourcesHelper in {Path.GetFileName(dll)}!");
            Console.WriteLine($"  Namespace: {rh.Namespace}");
            break;
        }
    }
    catch { }
}

$gamePath = 'D:\Program FIles\Steam\steamapps\common\LBoL\LBoL_Data\Managed'
$dlls = Get-ChildItem $gamePath -Filter '*.dll'
foreach ($dll in $dlls) { try { [System.Reflection.Assembly]::LoadFile($dll.FullName) | Out-Null } catch {} }
$flags = [System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::Static
$coreDll = Join-Path $gamePath 'LBoL.Core.dll'
$casm = [System.Reflection.Assembly]::LoadFile($coreDll)

Write-Host "=== LBoL.Core.Localization ==="
$locType = $casm.GetType('LBoL.Core.Localization')
if ($locType) {
    foreach ($p in $locType.GetProperties($flags)) { Write-Host "  Prop $($p.Name) (static=$($p.GetGetMethod($true).IsStatic)): $($p.PropertyType.Name)" }
    foreach ($f in $locType.GetFields($flags)) { Write-Host "  Field $($f.Name) (static=$($f.IsStatic)): $($f.FieldType.Name)" }
    foreach ($m in $locType.GetMethods($flags)) {
        if (-not $m.IsSpecialName) { Write-Host "  Method $($m.Name): $($m.ToString())" }
    }
}

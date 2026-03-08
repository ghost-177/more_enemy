$gamePath = 'D:\Program FIles\Steam\steamapps\common\LBoL\LBoL_Data\Managed'
$bepPath = 'D:\Program FIles\Steam\steamapps\common\LBoL\BepInEx\plugins\LBoL-Entity-Sideloader'
$dlls = Get-ChildItem $gamePath -Filter '*.dll'
foreach ($dll in $dlls) { try { [System.Reflection.Assembly]::LoadFile($dll.FullName) | Out-Null } catch {} }
$sideloaderDll = Join-Path $bepPath 'LBoL-Entity-Sideloader.dll'
$slasm = [System.Reflection.Assembly]::LoadFile($sideloaderDll)
$flags = [System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::Static
$allSlTypes = @()
try { $allSlTypes = $slasm.GetTypes() } catch [System.Reflection.ReflectionTypeLoadException] { $allSlTypes = $_.Exception.Types | Where-Object { $_ -ne $null } }

Write-Host "=== LBoL.Core.Locale enum values ==="
$coreDll = Join-Path $gamePath 'LBoL.Core.dll'
$casm = [System.Reflection.Assembly]::LoadFile($coreDll)
$localeType = $casm.GetType('LBoL.Core.Locale')
if ($localeType) {
    foreach ($v in [System.Enum]::GetValues($localeType)) { Write-Host "  $v = $([int]$v)" }
}

Write-Host ""
Write-Host "=== LocalizationOption type ==="
$locOptType = $allSlTypes | Where-Object { $_.Name -eq 'LocalizationOption' } | Select-Object -First 1
if ($locOptType) {
    Write-Host "Found: $($locOptType.FullName)"
    foreach ($f in $locOptType.GetFields($flags)) { Write-Host "  Field $($f.Name) (static=$($f.IsStatic)): $($f.FieldType.Name)" }
    foreach ($p in $locOptType.GetProperties($flags)) { Write-Host "  Prop $($p.Name) (static=$($p.GetGetMethod($true).IsStatic)): $($p.PropertyType.Name)" }
    foreach ($m in $locOptType.GetMethods($flags)) {
        if (-not $m.IsSpecialName) { Write-Host "  Method $($m.Name): $($m.ToString())" }
    }
}

Write-Host ""
Write-Host "=== LocalizationInfo type ==="
$locInfoType = $allSlTypes | Where-Object { $_.Name -eq 'LocalizationInfo' } | Select-Object -First 1
if ($locInfoType) {
    Write-Host "Found: $($locInfoType.FullName)"
    foreach ($f in $locInfoType.GetFields($flags)) { Write-Host "  Field $($f.Name) (static=$($f.IsStatic)): $($f.FieldType.Name)" }
    foreach ($p in $locInfoType.GetProperties($flags)) {
        try { Write-Host "  Prop $($p.Name) (static=$($p.GetGetMethod($true).IsStatic)): $($p.PropertyType.Name)" } catch {}
    }
}

Write-Host ""
Write-Host "=== LocalizationFiles constructor ==="
$locFilesType = $allSlTypes | Where-Object { $_.Name -eq 'LocalizationFiles' } | Select-Object -First 1
if ($locFilesType) {
    foreach ($c in $locFilesType.GetConstructors($flags)) {
        Write-Host "  Ctor: $($c.ToString())"
        foreach ($p in $c.GetParameters()) { Write-Host "    $($p.Name): $($p.ParameterType.FullName)" }
    }
}

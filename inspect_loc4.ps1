$gamePath = 'D:\Program FIles\Steam\steamapps\common\LBoL\LBoL_Data\Managed'
$bepPath = 'D:\Program FIles\Steam\steamapps\common\LBoL\BepInEx\plugins\LBoL-Entity-Sideloader'
$dlls = Get-ChildItem $gamePath -Filter '*.dll'
foreach ($dll in $dlls) { try { [System.Reflection.Assembly]::LoadFile($dll.FullName) | Out-Null } catch {} }
$sideloaderDll = Join-Path $bepPath 'LBoL-Entity-Sideloader.dll'
$slasm = [System.Reflection.Assembly]::LoadFile($sideloaderDll)
$flags = [System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::Static
$allSlTypes = @()
try { $allSlTypes = $slasm.GetTypes() } catch [System.Reflection.ReflectionTypeLoadException] { $allSlTypes = $_.Exception.Types | Where-Object { $_ -ne $null } }

# Look at LocalizationFiles.GetAvailableLocale IL to find how it detects current locale
Write-Host "=== LocalizationFiles.GetAvailableLocale IL ==="
$locFilesType = $allSlTypes | Where-Object { $_.Name -eq 'LocalizationFiles' } | Select-Object -First 1
$getAvailable = $locFilesType.GetMethod('GetAvailableLocale', $flags)
if ($getAvailable) {
    $body = $getAvailable.GetMethodBody()
    $bytes = $body.GetILAsByteArray()
    $module = $getAvailable.Module
    $i = 0
    while ($i -lt $bytes.Length) {
        $op = $bytes[$i]
        if ($op -eq 0x28 -or $op -eq 0x6F) {
            $token = [System.BitConverter]::ToInt32($bytes, $i+1)
            try { $m = $module.ResolveMethod($token); Write-Host "  $(if($op-eq0x28){'call'}else{'callvirt'}) $($m.DeclaringType.Name)::$($m.Name)" } catch { Write-Host "  call token $($token.ToString('X8'))" }
            $i += 5
        } elseif ($op -eq 0x72) {
            $token = [System.BitConverter]::ToInt32($bytes, $i+1)
            try { $s = $module.ResolveString($token); Write-Host "  ldstr `"$s`"" } catch {}
            $i += 5
        } elseif ($op -eq 0x7B -or $op -eq 0x7D -or $op -eq 0x7E -or $op -eq 0x80) { $i += 5
        } elseif ($op -eq 0xFE) { $i += 2
        } else { $i++ }
    }
}

# Look for locale detection in LBoL.Core
Write-Host ""
Write-Host "=== LBoL.Core types with Locale/Language in name ==="
$coreDll = Join-Path $gamePath 'LBoL.Core.dll'
$casm = [System.Reflection.Assembly]::LoadFile($coreDll)
try {
    foreach ($t in $casm.GetTypes()) {
        if ($t.Name -match 'Locale|Language|Localization') { Write-Host "  $($t.FullName)" }
    }
} catch [System.Reflection.ReflectionTypeLoadException] {
    foreach ($t in $_.Exception.Types | Where-Object { $_ -ne $null }) {
        if ($t.Name -match 'Locale|Language|Localization') { Write-Host "  $($t.FullName)" }
    }
}

# Check LBoL.Base for locale settings
Write-Host ""
Write-Host "=== LBoL.Base types with Locale/Language in name ==="
$baseDll = Join-Path $gamePath 'LBoL.Base.dll'
$basm = [System.Reflection.Assembly]::LoadFile($baseDll)
try {
    foreach ($t in $basm.GetTypes()) {
        if ($t.Name -match 'Locale|Language|Localization|Setting') { Write-Host "  $($t.FullName)" }
    }
} catch {}

# Check GameMaster for locale method
Write-Host ""
Write-Host "=== Sideloader HookPoints.Localization_Patch ==="
$patchType = $allSlTypes | Where-Object { $_.Name -eq 'Localization_Patch' } | Select-Object -First 1
if ($patchType) {
    foreach ($m in $patchType.GetMethods($flags)) {
        if (-not $m.IsSpecialName) { Write-Host "  $($m.Name): $($m.ToString())" }
    }
    foreach ($f in $patchType.GetFields($flags)) { Write-Host "  Field $($f.Name) (static=$($f.IsStatic)): $($f.FieldType.Name)" }
}

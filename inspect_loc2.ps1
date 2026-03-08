$gamePath = 'D:\Program FIles\Steam\steamapps\common\LBoL\LBoL_Data\Managed'
$bepPath = 'D:\Program FIles\Steam\steamapps\common\LBoL\BepInEx\plugins\LBoL-Entity-Sideloader'
$dlls = Get-ChildItem $gamePath -Filter '*.dll'
foreach ($dll in $dlls) { try { [System.Reflection.Assembly]::LoadFile($dll.FullName) | Out-Null } catch {} }
$sideloaderDll = Join-Path $bepPath 'LBoL-Entity-Sideloader.dll'
$slasm = [System.Reflection.Assembly]::LoadFile($sideloaderDll)
$flags = [System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::Static

# Get all types via partial load
$allSlTypes = @()
try { $allSlTypes = $slasm.GetTypes() } catch [System.Reflection.ReflectionTypeLoadException] { $allSlTypes = $_.Exception.Types | Where-Object { $_ -ne $null } }

Write-Host "=== LocalizationFiles type ==="
$locFilesType = $allSlTypes | Where-Object { $_.Name -eq 'LocalizationFiles' } | Select-Object -First 1
if ($locFilesType) {
    Write-Host "Found: $($locFilesType.FullName)"
    foreach ($m in $locFilesType.GetMethods($flags)) { Write-Host "  $($m.Name): $($m.ToString())" }
    foreach ($f in $locFilesType.GetFields($flags)) { Write-Host "  Field $($f.Name): $($f.FieldType.Name)" }
    foreach ($p in $locFilesType.GetProperties($flags)) { Write-Host "  Prop $($p.Name): $($p.PropertyType.Name)" }
}

Write-Host ""
Write-Host "=== DiscoverAndLoadLocFiles IL analysis ==="
$blType = $slasm.GetType('LBoLEntitySideloader.Resource.BatchLocalization')
$discover = $blType.GetMethod('DiscoverAndLoadLocFiles', $flags)
if ($discover) {
    $body = $discover.GetMethodBody()
    $bytes = $body.GetILAsByteArray()
    $module = $discover.Module
    $i = 0
    while ($i -lt $bytes.Length) {
        $op = $bytes[$i]
        if ($op -eq 0x28 -or $op -eq 0x6F) {
            $token = [System.BitConverter]::ToInt32($bytes, $i+1)
            try { $m = $module.ResolveMethod($token); Write-Host "  $(if($op-eq0x28){'call'}else{'callvirt'}) $($m.DeclaringType.Name)::$($m.Name)" } catch { Write-Host "  call token $($token.ToString('X8'))" }
            $i += 5
        } elseif ($op -eq 0x72) {  # ldstr
            $token = [System.BitConverter]::ToInt32($bytes, $i+1)
            try { $s = $module.ResolveString($token); Write-Host "  ldstr `"$s`"" } catch {}
            $i += 5
        } elseif ($op -eq 0x7B -or $op -eq 0x7D -or $op -eq 0x7E -or $op -eq 0x80 -or $op -eq 0xD0) { $i += 5
        } elseif ($op -eq 0xFE) { $i += 2
        } else { $i++ }
    }
}

Write-Host ""
Write-Host "=== Sideloader types matching Lang/Locale/Localiz ==="
foreach ($t in $allSlTypes) {
    if ($t.Name -match 'Lang|Locale|Localiz') { Write-Host "  $($t.FullName)" }
}

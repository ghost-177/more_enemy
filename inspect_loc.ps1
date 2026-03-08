$gamePath = 'D:\Program FIles\Steam\steamapps\common\LBoL\LBoL_Data\Managed'
$bepPath = 'D:\Program FIles\Steam\steamapps\common\LBoL\BepInEx\plugins\LBoL-Entity-Sideloader'
$dlls = Get-ChildItem $gamePath -Filter '*.dll'
foreach ($dll in $dlls) { try { [System.Reflection.Assembly]::LoadFile($dll.FullName) | Out-Null } catch {} }

$sideloaderDll = Join-Path $bepPath 'LBoL-Entity-Sideloader.dll'
$slasm = [System.Reflection.Assembly]::LoadFile($sideloaderDll)
$flags = [System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::Static

Write-Host "=== Sideloader types matching Local/Lang/Locale ==="
foreach ($t in $slasm.GetTypes()) {
    if ($t.Name -match 'Local|Lang|Locale') { Write-Host "  $($t.FullName)" }
}

Write-Host ""
Write-Host "=== BatchLocalization methods ==="
$blType = $slasm.GetType('LBoLEntitySideloader.Resource.BatchLocalization')
if ($blType) {
    foreach ($m in $blType.GetMethods($flags)) {
        Write-Host "  $($m.Name): $($m.ToString())"
    }
    Write-Host "Fields:"
    foreach ($f in $blType.GetFields($flags)) {
        Write-Host "  $($f.Name): $($f.FieldType.Name)"
    }
}

Write-Host ""
Write-Host "=== DirectorySource methods ==="
$dsType = $slasm.GetType('LBoLEntitySideloader.Resource.DirectorySource')
if ($dsType) {
    foreach ($m in $dsType.GetMethods($flags)) {
        if ($m.Name -match 'Lang|Loc|File|Load|Discover') {
            Write-Host "  $($m.Name): $($m.ToString())"
        }
    }
    foreach ($f in $dsType.GetFields($flags)) {
        Write-Host "  Field $($f.Name): $($f.FieldType.Name)"
    }
}

Write-Host ""
Write-Host "=== GameMaster locale/language properties ==="
$presentDll = Join-Path $gamePath 'LBoL.Presentation.dll'
$pasm = [System.Reflection.Assembly]::LoadFile($presentDll)
$allTypes = @()
try { $allTypes = $pasm.GetTypes() } catch [System.Reflection.ReflectionTypeLoadException] { $allTypes = $_.Exception.Types | Where-Object { $_ -ne $null } }
$gmType = $allTypes | Where-Object { $_.Name -eq 'GameMaster' } | Select-Object -First 1
if ($gmType) {
    foreach ($p in $gmType.GetProperties($flags)) {
        if ($p.Name -match 'Lang|Locale|Localiz') { Write-Host "  $($p.Name): $($p.PropertyType.Name)" }
    }
}

Write-Host ""
Write-Host "=== Searching all sideloader types for language utilities ==="
foreach ($t in $slasm.GetTypes()) {
    foreach ($m in $t.GetMethods($flags)) {
        if ($m.Name -match 'Lang|Locale|GetLang') {
            Write-Host "  $($t.Name)::$($m.Name): $($m.ToString())"
        }
    }
}

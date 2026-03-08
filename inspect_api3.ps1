[byte[]]$bytes = [System.IO.File]::ReadAllBytes('D:/Program Files/Steam/steamapps/common/LBoL/LBoL_Data/Managed/LBoL.Core.dll')
$sb = New-Object System.Text.StringBuilder
$cur = New-Object System.Text.StringBuilder
foreach ($b in $bytes) {
    if ($b -ge 65 -and $b -le 122) { [void]$cur.Append([char]$b) }
    else { if ($cur.Length -ge 4) { [void]$sb.AppendLine($cur.ToString()) }; $cur.Clear() | Out-Null }
}
$allWords = ($sb.ToString() -split "`n") | Where-Object { $_.Length -ge 4 } | Sort-Object -Unique

# Also check sideloader for ResourceLoader
[byte[]]$bytes2 = [System.IO.File]::ReadAllBytes('D:/Program Files/Steam/steamapps/common/LBoL/BepInEx/plugins/LBoL-Entity-Sideloader/LBoL-Entity-Sideloader.dll')
$sb2 = New-Object System.Text.StringBuilder
$cur2 = New-Object System.Text.StringBuilder
foreach ($b in $bytes2) {
    if ($b -ge 65 -and $b -le 122) { [void]$cur2.Append([char]$b) }
    else { if ($cur2.Length -ge 4) { [void]$sb2.AppendLine($cur2.ToString()) }; $cur2.Clear() | Out-Null }
}
$sideloaderWords = ($sb2.ToString() -split "`n") | Where-Object { $_.Length -ge 4 } | Sort-Object -Unique

Write-Host "=== SelectCards / GainCards / SelectCard in Core ==="
$allWords | Where-Object { $_ -match 'SelectCard|GainCard|SelectCards' }

Write-Host "=== ResourceLoader in Sideloader ==="
$sideloaderWords | Where-Object { $_ -match 'ResourceLoader|LoadSprite' }

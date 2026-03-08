# Check Adventure methods
[byte[]]$bytes = [System.IO.File]::ReadAllBytes('D:/Program Files/Steam/steamapps/common/LBoL/LBoL_Data/Managed/LBoL.Core.dll')
$sb = New-Object System.Text.StringBuilder
$cur = New-Object System.Text.StringBuilder
foreach ($b in $bytes) {
    if ($b -ge 65 -and $b -le 122) { [void]$cur.Append([char]$b) }
    else { if ($cur.Length -ge 4) { [void]$sb.AppendLine($cur.ToString()) }; $cur.Clear() | Out-Null }
}
$lines = $sb.ToString() -split "`n"
# Check Base dll too
[byte[]]$bytes2 = [System.IO.File]::ReadAllBytes('D:/Program Files/Steam/steamapps/common/LBoL/LBoL_Data/Managed/LBoL.Base.dll')
$sb2 = New-Object System.Text.StringBuilder
$cur2 = New-Object System.Text.StringBuilder
foreach ($b in $bytes2) {
    if ($b -ge 65 -and $b -le 122) { [void]$cur2.Append([char]$b) }
    else { if ($cur2.Length -ge 4) { [void]$sb2.AppendLine($cur2.ToString()) }; $cur2.Clear() | Out-Null }
}
$lines2 = $sb2.ToString() -split "`n"

Write-Host "=== Library/Card related in Core ==="
$lines | Where-Object { $_ -match 'Library|TryCreate|SelectCard' } | Sort-Object -Unique

Write-Host "=== Library/Card related in Base ==="
$lines2 | Where-Object { $_ -match 'Library|TryCreate|SelectCard' } | Sort-Object -Unique

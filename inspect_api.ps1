[byte[]]$bytes = [System.IO.File]::ReadAllBytes('D:/Program Files/Steam/steamapps/common/LBoL/LBoL_Data/Managed/LBoL.Core.dll')
$sb = New-Object System.Text.StringBuilder
$cur = New-Object System.Text.StringBuilder
foreach ($b in $bytes) {
    if ($b -ge 65 -and $b -le 122) { [void]$cur.Append([char]$b) }
    else { if ($cur.Length -ge 4) { [void]$sb.AppendLine($cur.ToString()) }; $cur.Clear() | Out-Null }
}
$lines = $sb.ToString() -split "`n"
$lines | Where-Object { $_ -match 'RollExhibit|GetSpecial|SelectCard|GainExhibit|EnumerateRoll|CreateCard|AdventureFlow|Library' } | Sort-Object -Unique

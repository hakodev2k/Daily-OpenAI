$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
dotnet restore $project | Out-Null
$output = dotnet run --project $project
Write-Host $output

$planLines = @($output | Select-String '^Plan=(.+)$')
$countMatch = $output | Select-String '^Count=(\d+)$'
if (-not $planLines -or -not $countMatch) { Write-Error "FAIL: missing expected diagnostics."; exit 1 }
$count = [int]$countMatch.Matches.Groups[1].Value
if ($count -ne 1440) { Write-Error "FAIL: expected Count=1440 but got $count."; exit 1 }
$planText = ($planLines | ForEach-Object { $_.Matches.Groups[1].Value }) -join " "
if ($planText -notmatch 'SEARCH Orders USING') { Write-Error "FAIL: query plan is not using an index search: $planText"; exit 1 }
Write-Host "PASS: semantics preserved and execution plan uses index search."

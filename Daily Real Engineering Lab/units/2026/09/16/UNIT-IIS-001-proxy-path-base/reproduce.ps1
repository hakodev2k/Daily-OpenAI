$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Portal.csproj'
$root = dotnet run --project $project -- ''
$sub = dotnet run --project $project -- '/ops'
Write-Host "Root: $root"
Write-Host "Sub-path: $sub"
if ($root -ne '/reports/42') { throw 'Unexpected root-hosting result.' }
if ($sub -eq '/ops/reports/42') { throw 'Starter no longer reproduces the intended symptom.' }
Write-Host 'Reproduced: root hosting works while sub-path hosting loses its application prefix.'
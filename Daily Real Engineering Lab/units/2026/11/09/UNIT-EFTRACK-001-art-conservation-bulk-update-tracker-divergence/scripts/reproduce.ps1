$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/ConservationLab.csproj'
$output = & dotnet run --project $project -- reproduce 2>&1
if ($LASTEXITCODE -ne 0) { throw ($output -join [Environment]::NewLine) }
$output | ForEach-Object { Write-Output $_ }
if (-not (($output -join ' ') -match 'dbAfterBulk=5' -and ($output -join ' ') -match 'finalPriority=1')) { throw 'Expected starter symptom not observed; reproduce must run before editing.' }

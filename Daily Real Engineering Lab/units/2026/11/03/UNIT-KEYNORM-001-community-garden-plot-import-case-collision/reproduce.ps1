$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/GardenImport.csproj'
$output = & dotnet run --project $project
if ($LASTEXITCODE -ne 0) { throw "Starter execution failed" }
$output | ForEach-Object { Write-Output $_ }
if (($output -match '^InputRows=4$').Count -ne 1 -or ($output -match '^StoredPlots=3$').Count -ne 1 -or -not ($output -match 'Owners=')) { throw 'Expected starter symptom not reproduced' }
Write-Output 'REPRODUCED: 4 input rows but only 3 stored plots.'

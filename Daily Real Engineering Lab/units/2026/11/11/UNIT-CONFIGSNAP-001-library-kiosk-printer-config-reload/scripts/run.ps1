$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/Kiosk.csproj'
dotnet run --project $project
if ($LASTEXITCODE -ne 0) { throw 'Starter failed to run' }


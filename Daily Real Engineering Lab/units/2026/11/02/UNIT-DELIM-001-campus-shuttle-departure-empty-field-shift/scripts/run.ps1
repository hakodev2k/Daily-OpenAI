$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/Starter.csproj'
& dotnet run --project $project -- --demo
if ($LASTEXITCODE -ne 0) { throw 'Starter demo failed; check .NET SDK installation.' }

$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/PermitRegistryLab.csproj"
dotnet run --project $project -- --diagnose
if ($LASTEXITCODE -ne 0) { throw "Diagnostic run failed with exit code $LASTEXITCODE" }

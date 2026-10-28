$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/PermitRegistryLab.csproj"
dotnet run --project $project -- --verify
if ($LASTEXITCODE -ne 0) { throw "Learner-edited starter failed verification (exit code $LASTEXITCODE)." }

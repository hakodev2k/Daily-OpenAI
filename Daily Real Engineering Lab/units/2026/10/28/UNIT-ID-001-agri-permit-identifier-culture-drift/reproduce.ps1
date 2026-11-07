$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/PermitRegistryLab.csproj"
dotnet run --project $project -- --reproduce
if ($LASTEXITCODE -ne 0) { throw "Starter symptom was not reproduced (exit code $LASTEXITCODE)." }

$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/RescueDecoder.csproj') -- reproduce
if ($LASTEXITCODE -ne 0) { throw "Starter symptom not reproduced (exit $LASTEXITCODE)." }

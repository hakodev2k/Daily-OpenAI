$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ResultDispatchGate.csproj" -- --reproduce
if ($LASTEXITCODE -ne 0) { throw "Reproduction failed" }

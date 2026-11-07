$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ResultDispatchGate.csproj" -- --verify
if ($LASTEXITCODE -ne 0) { throw "Learner verification failed" }

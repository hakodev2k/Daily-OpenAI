$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/SeatLab.csproj" -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed for learner starter code.' }

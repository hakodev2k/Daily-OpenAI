$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/MuseumLab.csproj" -- verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: response/database/pending-edit contract.' }

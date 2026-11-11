$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/starter/ArchiveReplay.csproj" -- --verify
if ($LASTEXITCODE -ne 0) { throw "Learner regression verification failed" }

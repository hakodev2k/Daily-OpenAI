$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/starter/ArchiveReplay.csproj" -- --reproduce
if ($LASTEXITCODE -ne 0) { throw "Symptom not reproduced" }

$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/starter/TicketReplay.csproj" -- --reproduce
if ($LASTEXITCODE -ne 0) { throw "Symptom not reproduced" }

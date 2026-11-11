$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/starter/TicketReplay.csproj" -- --verify
if ($LASTEXITCODE -ne 0) { throw "Learner regression verification failed" }

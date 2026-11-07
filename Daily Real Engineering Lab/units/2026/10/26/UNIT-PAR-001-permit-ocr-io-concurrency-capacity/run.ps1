$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/starter/PermitOcrBatch.csproj" -- --max-concurrency 8
exit $LASTEXITCODE

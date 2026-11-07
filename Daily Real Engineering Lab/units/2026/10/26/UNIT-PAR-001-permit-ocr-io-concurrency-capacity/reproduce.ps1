$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/harness/Verifier.csproj" -- reproduce "$PSScriptRoot/starter/PermitOcrBatch.csproj"
exit $LASTEXITCODE

$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/harness/Verifier.csproj" -- verify "$PSScriptRoot/starter/PermitOcrBatch.csproj"
exit $LASTEXITCODE

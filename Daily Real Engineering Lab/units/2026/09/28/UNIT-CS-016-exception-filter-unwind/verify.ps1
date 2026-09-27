$ErrorActionPreference="Stop"
dotnet run --project ./starter/ExceptionTimeline.csproj -- --verify
if($LASTEXITCODE -ne 0){throw "Verification failed: state-dependent policy still runs before cleanup."}

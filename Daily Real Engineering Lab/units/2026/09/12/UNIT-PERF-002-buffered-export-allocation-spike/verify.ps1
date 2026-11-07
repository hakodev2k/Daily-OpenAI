$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

dotnet run --project (Join-Path $root 'verify/Verify.csproj') -c Release
if ($LASTEXITCODE -ne 0) {
    throw "Verification failed with exit code $LASTEXITCODE."
}

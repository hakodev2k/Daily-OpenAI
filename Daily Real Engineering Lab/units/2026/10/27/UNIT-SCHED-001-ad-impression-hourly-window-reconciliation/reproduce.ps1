$ErrorActionPreference = 'Stop'
# Exit code 0 means the original failure was reproduced, not that the fix passed.
& dotnet run --project (Join-Path $PSScriptRoot 'starter/AdWindowLab.csproj') -- reproduce
exit $LASTEXITCODE

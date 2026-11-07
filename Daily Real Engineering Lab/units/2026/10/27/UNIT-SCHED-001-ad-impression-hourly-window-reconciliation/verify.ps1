$ErrorActionPreference = 'Stop'
# Verify the learner-edited starter, never only the reference solution.
& dotnet run --project (Join-Path $PSScriptRoot 'starter/AdWindowLab.csproj') -- verify
exit $LASTEXITCODE

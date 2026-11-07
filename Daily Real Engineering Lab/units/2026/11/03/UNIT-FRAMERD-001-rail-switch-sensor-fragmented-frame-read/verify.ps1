$ErrorActionPreference = 'Stop'
# Verifies the learner-editable starter/FrameDecoder.cs, not the reference solution.
dotnet run --project (Join-Path $PSScriptRoot 'starter/FrameLab.csproj') -- verify
exit $LASTEXITCODE

$ErrorActionPreference = 'Stop'
# Frozen baseline in Program.cs: edits to FrameDecoder.cs do not change reproduction.
dotnet run --project (Join-Path $PSScriptRoot 'starter/FrameLab.csproj') -- reproduce
exit $LASTEXITCODE

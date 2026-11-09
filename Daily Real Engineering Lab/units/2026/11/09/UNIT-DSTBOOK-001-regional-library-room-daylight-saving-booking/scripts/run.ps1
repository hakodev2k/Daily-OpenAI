$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/DstBooking.csproj') -- '2026-11-02 10:00' 'America/New_York' '-'
if ($LASTEXITCODE -ne 0) { throw 'run failed' }

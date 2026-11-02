$ErrorActionPreference = 'Stop'
$unit = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $unit 'harness/Contracts.csproj') -- verify
exit $LASTEXITCODE

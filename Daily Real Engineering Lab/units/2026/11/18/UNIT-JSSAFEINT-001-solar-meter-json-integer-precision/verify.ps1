$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/MeterReplay.csproj'
$output = & dotnet run --project $project
if ($LASTEXITCODE -ne 0) { throw "Learner starter failed to run" }
$output | ForEach-Object { Write-Host $_ }
if (-not ($output -match '^DISTINCT_CLIENT_IDS=2$')) { throw "Two IDs must remain distinct" }
if (-not ($output -match '^ID_ROUNDTRIP_OK=true$')) { throw "Identifier round-trip failed" }
if (-not ($output -match '^KWH_NUMERIC=true$')) { throw "Kwh must remain a JSON number" }
if (-not ($output -match '^WIRE=.*"Id":"9007199254740993"')) { throw "Wire ID must be a JSON string preserving the original digits" }
Write-Host 'PASS: learner starter satisfies identifier and measurement contracts.'


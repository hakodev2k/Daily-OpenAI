$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = dotnet run --project (Join-Path $root 'starter/ExportLab.csproj') -c Release
$output | ForEach-Object { Write-Host $_ }

$payloadLine = $output | Where-Object { $_ -match '^PAYLOAD_BYTES=' } | Select-Object -Last 1
$allocatedLine = $output | Where-Object { $_ -match '^ALLOCATED_BYTES=' } | Select-Object -Last 1
if (-not $payloadLine -or -not $allocatedLine) {
    throw 'Could not parse allocation evidence from starter output.'
}

$payload = [int64](($payloadLine -split '=')[1])
$allocated = [int64](($allocatedLine -split '=')[1])

if ($allocated -lt [int64]($payload * 0.75)) {
    throw "Symptom not reproduced: allocated=$allocated payload=$payload. Starter may already be fixed."
}

Write-Host "REPRODUCED: export allocated $allocated bytes for a $payload-byte payload."

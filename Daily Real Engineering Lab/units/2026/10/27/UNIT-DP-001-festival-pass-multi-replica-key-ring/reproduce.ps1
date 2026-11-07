$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$project = Join-Path $PSScriptRoot 'src/LabHarness/LabHarness.csproj'
$root = Join-Path ([System.IO.Path]::GetTempPath()) ('festival-pass-repro-' + [guid]::NewGuid().ToString('N'))
$token = Join-Path $root 'session.txt'
New-Item -ItemType Directory -Path $root -Force | Out-Null
try {
    dotnet run --project $project -- mint $root $token
    if ($LASTEXITCODE -ne 0) { throw 'Mint failed; this is not a valid reproduction.' }

    dotnet run --project $project -- read $root $token replica-b
    if ($LASTEXITCODE -ne 21) { throw "Expected cryptographic rejection at replica B (exit 21), got $LASTEXITCODE." }

    dotnet run --project $project -- read $root $token replica-a
    if ($LASTEXITCODE -ne 21) { throw "Expected rejection after A process restart (exit 21), got $LASTEXITCODE." }

    Write-Host 'REPRODUCED: both independent processes rejected a valid minted session.'
} finally {
    Remove-Item -LiteralPath $root -Recurse -Force -ErrorAction SilentlyContinue
}

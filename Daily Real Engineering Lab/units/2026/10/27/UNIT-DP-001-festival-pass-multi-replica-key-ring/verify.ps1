$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$project = Join-Path $PSScriptRoot 'src/LabHarness/LabHarness.csproj'
$root = Join-Path ([System.IO.Path]::GetTempPath()) ('festival-pass-verify-' + [guid]::NewGuid().ToString('N'))
$other = Join-Path ([System.IO.Path]::GetTempPath()) ('festival-pass-isolated-' + [guid]::NewGuid().ToString('N'))
$token = Join-Path $root 'session.txt'
New-Item -ItemType Directory -Path $root -Force | Out-Null
New-Item -ItemType Directory -Path $other -Force | Out-Null
try {
    dotnet run --project $project -- mint $root $token
    if ($LASTEXITCODE -ne 0) { throw 'Mint failed.' }

    dotnet run --project $project -- read $root $token replica-b
    if ($LASTEXITCODE -ne 0) { throw 'Cross-replica session must be accepted.' }

    dotnet run --project $project -- read $root $token replica-a
    if ($LASTEXITCODE -ne 0) { throw 'New process of replica A must accept the session.' }

    dotnet run --project $project -- read $root $token replica-b festival.admin-session.v1
    if ($LASTEXITCODE -ne 21) { throw 'Purpose isolation failed: wrong purpose must be rejected (exit 21).' }

    dotnet run --project $project -- read $other $token replica-b
    if ($LASTEXITCODE -ne 21) { throw 'Key-ring isolation failed: other directory must be rejected (exit 21).' }

    Write-Host 'VERIFY PASS: cross-replica and restart accepted; wrong purpose and isolated ring rejected.'
} finally {
    Remove-Item -LiteralPath $root -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item -LiteralPath $other -Recurse -Force -ErrorAction SilentlyContinue
}

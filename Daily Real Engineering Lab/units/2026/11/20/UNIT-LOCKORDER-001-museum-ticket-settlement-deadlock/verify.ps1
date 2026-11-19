$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TicketSettlement.csproj'
dotnet build $project -c Release --nologo | Out-Host
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
$dll = Join-Path $PSScriptRoot 'starter/bin/Release/net8.0/TicketSettlement.dll'
$out = Join-Path $env:TEMP ("ticket-verify-" + [guid]::NewGuid() + ".txt")
$err = Join-Path $env:TEMP ("ticket-verify-" + [guid]::NewGuid() + ".err")
try {
  $p = Start-Process dotnet -ArgumentList @($dll) -PassThru -RedirectStandardOutput $out -RedirectStandardError $err
  try {
    if (-not $p.WaitForExit(5000)) { throw 'FAIL: process still blocked after 5 seconds' }
    if ($p.ExitCode -ne 0) { throw ("FAIL: exit " + $p.ExitCode + ": " + (Get-Content $err -Raw)) }
    $result = Get-Content $out -Raw
    if ($result -notmatch 'SETTLED total=200 left=110 right=90') { throw ("FAIL: settlement invariant or expected transfers: " + $result) }
    Write-Host 'PASS: both transfers completed; balances and conservation invariant hold.'
  } finally { if (-not $p.HasExited) { Stop-Process -Id $p.Id -Force } }
} finally { Remove-Item $out,$err -Force -ErrorAction SilentlyContinue }


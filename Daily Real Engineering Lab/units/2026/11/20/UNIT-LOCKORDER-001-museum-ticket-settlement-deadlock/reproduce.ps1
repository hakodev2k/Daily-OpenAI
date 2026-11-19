$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TicketSettlement.csproj'
dotnet build $project -c Release --nologo | Out-Host
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
$dll = Join-Path $PSScriptRoot 'starter/bin/Release/net8.0/TicketSettlement.dll'
$p = Start-Process dotnet -ArgumentList @($dll) -PassThru -NoNewWindow
try {
  if ($p.WaitForExit(5000)) { throw "Expected blocked process; observed exit $($p.ExitCode)" }
  Write-Host 'REPRODUCED: settlement did not finish within 5 seconds.'
} finally {
  if (-not $p.HasExited) { Stop-Process -Id $p.Id -Force }
}


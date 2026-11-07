$ErrorActionPreference = 'Stop'
$source = Join-Path $PSScriptRoot 'starter/TicketPricingService.cs'
$original = Join-Path $PSScriptRoot 'starter/TicketPricingService.original.txt'
if (-not (Test-Path $original)) { throw 'Missing immutable starter baseline.' }
$temp = Join-Path ([System.IO.Path]::GetTempPath()) ('museum-lab-' + [guid]::NewGuid().ToString('N'))
try {
  New-Item -ItemType Directory -Path $temp | Out-Null
  Copy-Item (Join-Path $PSScriptRoot 'starter/*') $temp -Recurse
  Copy-Item $original (Join-Path $temp 'TicketPricingService.cs') -Force
  dotnet run --project (Join-Path $temp 'MuseumLab.csproj') -- reproduce
  if ($LASTEXITCODE -ne 0) { throw 'Expected starter symptom not reproduced.' }
} finally { Remove-Item $temp -Recurse -Force -ErrorAction SilentlyContinue }

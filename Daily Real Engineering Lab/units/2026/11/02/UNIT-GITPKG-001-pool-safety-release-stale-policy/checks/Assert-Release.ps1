param([Parameter(Mandatory=$true)][ValidateSet('broken','fixed')][string]$Mode)
$ErrorActionPreference = 'Stop'
$work = Join-Path (Split-Path $PSScriptRoot -Parent) '.lab-work'
$out = Join-Path $work 'artifact'
$package = Join-Path $out 'package'
if (-not (Test-Path -LiteralPath $package)) { throw 'Missing package directory' }
$trackedPaths = @(& git -C $work ls-files -- 'release/*.json')
if ($LASTEXITCODE -ne 0) { throw 'git ls-files failed' }
$expected = @($trackedPaths | ForEach-Object { Split-Path $_ -Leaf } | Sort-Object -Unique)
$actual = @(Get-ChildItem -LiteralPath $package -File -Filter '*.json' | ForEach-Object { $_.Name } | Sort-Object)
$manifest = Get-Content -LiteralPath (Join-Path $out 'manifest.json') -Raw | ConvertFrom-Json
$listed = @($manifest.files | Sort-Object)
$missing = @($expected | Where-Object { $_ -notin $actual })
$unexpected = @($actual | Where-Object { $_ -notin $expected })
if (($listed -join '|') -ne ($actual -join '|')) { throw 'Manifest and packaged files disagree' }
foreach ($name in $expected) {
  if ($name -in $actual) {
    $source = Get-Content -LiteralPath (Join-Path $work "release/$name") -Raw
    $packed = Get-Content -LiteralPath (Join-Path $package $name) -Raw
    if ($source -cne $packed) { throw "Payload mismatch for $name" }
  }
}
Write-Host "Tracked=$($expected.Count) Packaged=$($actual.Count) Missing=$($missing.Count) Unexpected=$($unexpected.Count)"
if ($Mode -eq 'broken') {
  if ($missing.Count -ne 0 -or $unexpected.Count -lt 2) { throw 'Expected artifact contamination was not reproduced' }
  Write-Host 'REPRODUCED: artifact includes files outside approved Git revision'
} else {
  if ($missing.Count -ne 0 -or $unexpected.Count -ne 0) { throw 'Artifact does not match approved tracked policy set' }
  Write-Host 'VERIFIED: artifact and manifest contain exactly the approved policy set'
}

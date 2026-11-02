param(
  [Parameter(Mandatory=$true)][string]$Workspace,
  [Parameter(Mandatory=$true)][string]$Output
)
$ErrorActionPreference = 'Stop'
$release = Join-Path $Workspace 'release'
if (-not (Test-Path -LiteralPath $release -PathType Container)) { throw 'Missing release directory' }

# Git index represents the approved source file set for this lab.
$tracked = @(& git -C $Workspace ls-files -- 'release/*.json')
if ($LASTEXITCODE -ne 0) { throw 'Unable to resolve approved Git file list' }
$names = @($tracked | ForEach-Object { Split-Path $_ -Leaf } | Sort-Object -Unique)
if ($names.Count -eq 0) { throw 'Approved policy list is empty' }
foreach ($name in $names) {
  if (-not (Test-Path -LiteralPath (Join-Path $release $name) -PathType Leaf)) {
    throw "Tracked policy missing from checkout: $name"
  }
}

if (Test-Path -LiteralPath $Output) { Remove-Item -LiteralPath $Output -Recurse -Force }
$package = Join-Path $Output 'package'
New-Item -ItemType Directory -Path $package -Force | Out-Null
foreach ($name in $names) {
  Copy-Item -LiteralPath (Join-Path $release $name) -Destination (Join-Path $package $name)
}
[ordered]@{ files = @($names) } | ConvertTo-Json -Depth 4 |
  Set-Content -LiteralPath (Join-Path $Output 'manifest.json') -Encoding UTF8
Write-Host "Packaged $($names.Count) approved policy files into $Output"

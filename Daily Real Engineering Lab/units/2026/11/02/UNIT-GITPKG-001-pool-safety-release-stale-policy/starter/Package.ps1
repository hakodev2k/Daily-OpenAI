param(
  [Parameter(Mandatory=$true)][string]$Workspace,
  [Parameter(Mandatory=$true)][string]$Output
)
$ErrorActionPreference = 'Stop'
$release = Join-Path $Workspace 'release'
if (-not (Test-Path -LiteralPath $release -PathType Container)) { throw 'Missing release directory' }
if (Test-Path -LiteralPath $Output) { Remove-Item -LiteralPath $Output -Recurse -Force }
$package = Join-Path $Output 'package'
New-Item -ItemType Directory -Path $package -Force | Out-Null

# Legacy packaging convention: include all JSON files in the release directory.
$files = @(Get-ChildItem -LiteralPath $release -Filter '*.json' -File | Sort-Object Name)
foreach ($file in $files) {
  Copy-Item -LiteralPath $file.FullName -Destination (Join-Path $package $file.Name)
}
$manifest = [ordered]@{ files = @($files | ForEach-Object { $_.Name }) }
$manifest | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $Output 'manifest.json') -Encoding UTF8
Write-Host "Packaged $($files.Count) policy files into $Output"

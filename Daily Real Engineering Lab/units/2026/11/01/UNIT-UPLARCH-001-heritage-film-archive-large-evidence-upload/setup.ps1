$ErrorActionPreference = 'Stop'
if ($PSVersionTable.PSVersion.Major -lt 5) { throw 'PowerShell 5.1+ is required.' }
Get-Content -LiteralPath (Join-Path $PSScriptRoot 'starter/constraints.json') -Raw | ConvertFrom-Json | Out-Null
Get-Content -LiteralPath (Join-Path $PSScriptRoot 'starter/architecture.json') -Raw | ConvertFrom-Json | Out-Null
Write-Host ('SETUP OK: PowerShell ' + $PSVersionTable.PSVersion.ToString() + '; no external services needed.')

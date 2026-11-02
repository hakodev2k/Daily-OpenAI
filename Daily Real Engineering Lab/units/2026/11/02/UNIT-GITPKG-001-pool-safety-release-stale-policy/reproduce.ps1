$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'setup.ps1')
& (Join-Path $PSScriptRoot 'run.ps1')
& (Join-Path $PSScriptRoot 'checks/Assert-Release.ps1') -Mode broken

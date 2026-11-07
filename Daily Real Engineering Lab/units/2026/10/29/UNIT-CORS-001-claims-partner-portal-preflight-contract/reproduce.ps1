$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $root 'setup.ps1')
& (Join-Path $root 'scripts/probe.ps1') -Mode 'reproduce'

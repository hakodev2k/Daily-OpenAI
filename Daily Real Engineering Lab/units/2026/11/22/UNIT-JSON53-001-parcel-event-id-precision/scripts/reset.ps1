$ErrorActionPreference = 'Stop'
Copy-Item (Join-Path $PSScriptRoot '../solution/ParcelEvents.csproj') (Join-Path $PSScriptRoot '../starter/ParcelEvents.csproj') -WhatIf
Write-Host 'Reset manually with git restore -- starter/Program.cs (never overwrites work automatically).'

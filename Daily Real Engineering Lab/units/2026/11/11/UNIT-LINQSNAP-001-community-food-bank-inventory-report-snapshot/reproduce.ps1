$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/InventoryLab.csproj') -- reproduce
if ($LASTEXITCODE -ne 0) { throw 'Expected mismatch not reproduced' }

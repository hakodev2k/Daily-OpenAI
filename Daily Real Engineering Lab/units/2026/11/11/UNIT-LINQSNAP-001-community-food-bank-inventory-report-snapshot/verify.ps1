$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/InventoryLab.csproj') -- verify
if ($LASTEXITCODE -ne 0) { throw 'Learner code failed consistency contract' }

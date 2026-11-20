$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/GrantPolicyLab.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Starter failed' }

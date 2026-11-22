$ErrorActionPreference = 'Stop'
dotnet restore (Join-Path $PSScriptRoot '../starter/PermitDeadlineLab.csproj')
if ($LASTEXITCODE -ne 0) { exit 1 }

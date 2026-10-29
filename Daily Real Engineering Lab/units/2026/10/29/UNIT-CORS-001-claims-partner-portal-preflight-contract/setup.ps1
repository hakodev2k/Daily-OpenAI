$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet restore 'starter/ClaimsApi.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'Restore failed' }
    dotnet build 'starter/ClaimsApi.csproj' --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
    Write-Host 'SETUP OK'
} finally { Pop-Location }

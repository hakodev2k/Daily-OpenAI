$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project 'starter/ClaimsApi.csproj' -- --urls 'http://127.0.0.1:5087'
    if ($LASTEXITCODE -ne 0) { throw 'Run failed' }
} finally { Pop-Location }

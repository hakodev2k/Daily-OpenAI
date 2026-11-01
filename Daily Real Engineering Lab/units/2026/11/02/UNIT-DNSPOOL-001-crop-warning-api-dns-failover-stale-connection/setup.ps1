$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet --version
    dotnet restore ./starter/DnsPoolLab.csproj
    if ($LASTEXITCODE -ne 0) { throw 'Starter restore failed.' }
    dotnet build ./starter/DnsPoolLab.csproj --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'Starter build failed.' }
    dotnet restore ./solution/DnsPoolLab.csproj
    if ($LASTEXITCODE -ne 0) { throw 'Reference restore failed.' }
    dotnet build ./solution/DnsPoolLab.csproj --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'Reference build failed.' }
} finally { Pop-Location }

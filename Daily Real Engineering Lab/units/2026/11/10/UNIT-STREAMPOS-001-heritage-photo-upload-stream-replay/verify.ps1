$ErrorActionPreference='Stop'
$p=Join-Path $PSScriptRoot 'starter/ArchiveLab.csproj'
dotnet build $p -c Release
if ($LASTEXITCODE -ne 0) { exit 1 }
dotnet run --no-build -c Release --project $p
if ($LASTEXITCODE -ne 0) { Write-Error 'Learner code still fails'; exit 1 }
Write-Host 'VERIFY PASS'

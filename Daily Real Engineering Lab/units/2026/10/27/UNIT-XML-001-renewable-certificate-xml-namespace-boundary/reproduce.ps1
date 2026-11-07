$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CertificateLab/CertificateLab.csproj'
dotnet run --project $project -- reproduce
exit $LASTEXITCODE

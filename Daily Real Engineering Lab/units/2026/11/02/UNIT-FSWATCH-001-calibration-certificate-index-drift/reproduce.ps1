$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CertificateIndexLab.csproj'
dotnet run --project $project --no-launch-profile
$exitCode = $LASTEXITCODE
if ($exitCode -ne 3) {
    Write-Error "Expected original starter exit code 3, got $exitCode. If already fixed, use verify.ps1."
    exit 1
}
Write-Host 'Reproduction confirmed: certificate index diverges from directory.'
exit 0

$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/SecretRotationLab.csproj'

dotnet run --project $project
$exitCode = $LASTEXITCODE

if ($exitCode -eq 0) {
    throw 'Expected the starter to reproduce the post-rotation authentication failure, but it exited successfully.'
}

Write-Host 'Reproduction succeeded: starter demonstrated the intended post-rotation failure.'
exit 0

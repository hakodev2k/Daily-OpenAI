$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/TicketGateway/TicketGateway.csproj'
dotnet build $project --configuration Release
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
dotnet run --no-build --configuration Release --project $project
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: fix the learner starter code.' }
Write-Host 'Verification passed.'

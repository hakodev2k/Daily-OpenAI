$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/TicketGateway/TicketGateway.csproj'
dotnet run --project $project
if ($LASTEXITCODE -eq 0) { throw 'Reproduction unexpectedly passed; confirm starter remains unchanged.' }
Write-Host 'Reproduction confirmed: expected failing cases.'

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/FormValidator.csproj'
dotnet run --project $project -- normal
dotnet run --project $project -- adversarial
if ($LASTEXITCODE -ne 0) { throw 'Reproduction failed to execute.' }
Write-Host 'Record elapsedMs for both inputs. This script demonstrates symptoms; it does not certify a fix.'

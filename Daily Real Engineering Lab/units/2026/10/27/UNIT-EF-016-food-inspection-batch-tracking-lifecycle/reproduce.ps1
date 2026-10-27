$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project ./starter/InspectionLab.csproj -- reproduce
  if ($LASTEXITCODE -ne 0) { throw 'Reproduction did not observe the original failure.' }
} finally { Pop-Location }

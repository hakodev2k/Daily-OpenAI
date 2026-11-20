$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet run --project starter/CatalogLab.csproj -- verify
  if ($LASTEXITCODE -ne 0) { throw 'Verification failed: edit starter/Program.cs and retry' }
} finally { Pop-Location }

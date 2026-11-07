$ErrorActionPreference = 'Stop'
dotnet restore ./starter/Lab.csproj
dotnet run --project ./starter/Lab.csproj
if ($LASTEXITCODE -eq 0) { throw 'Expected the starter update operation to fail, but it completed.' }
Write-Host 'Reproduction confirmed: starter update operation failed as expected.'
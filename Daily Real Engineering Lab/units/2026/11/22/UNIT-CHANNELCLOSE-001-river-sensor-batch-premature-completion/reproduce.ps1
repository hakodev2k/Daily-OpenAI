$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/RiverBatch.csproj"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

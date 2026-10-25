$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/BaggageDispatch.csproj'
dotnet run --project $project -- verify
exit $LASTEXITCODE

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'starter/Lab.csproj'
dotnet run --project $project
exit $LASTEXITCODE
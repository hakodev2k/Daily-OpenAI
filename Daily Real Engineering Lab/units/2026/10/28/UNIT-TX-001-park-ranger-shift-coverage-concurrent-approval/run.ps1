$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'starter/ShiftLeaveLab.csproj'
dotnet run --project $project --configuration Release -- --demo
exit $LASTEXITCODE

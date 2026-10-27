$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'starter/ShiftLeaveLab.csproj'
dotnet --version
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore $project
exit $LASTEXITCODE

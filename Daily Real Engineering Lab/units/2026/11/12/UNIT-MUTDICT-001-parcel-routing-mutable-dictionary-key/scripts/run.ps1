$root = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $root 'starter/ParcelLab.csproj')
exit $LASTEXITCODE

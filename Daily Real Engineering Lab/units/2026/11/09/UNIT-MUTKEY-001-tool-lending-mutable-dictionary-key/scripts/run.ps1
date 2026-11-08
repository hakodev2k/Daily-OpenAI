$root = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $root 'starter/Lab.csproj')
exit $LASTEXITCODE

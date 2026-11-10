$unit = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $unit 'tests/Contract/Contract.csproj')
exit $LASTEXITCODE

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
& dotnet run --project (Join-Path $root 'starter/BulkLab.csproj') --configuration Release
exit $LASTEXITCODE

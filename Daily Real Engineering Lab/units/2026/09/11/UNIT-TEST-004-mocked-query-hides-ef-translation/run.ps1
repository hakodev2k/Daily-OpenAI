$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/Lab.csproj" -- compare
exit $LASTEXITCODE
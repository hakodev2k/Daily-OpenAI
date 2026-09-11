$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

dotnet run --project "$root/starter/Lab.csproj"
exit $LASTEXITCODE

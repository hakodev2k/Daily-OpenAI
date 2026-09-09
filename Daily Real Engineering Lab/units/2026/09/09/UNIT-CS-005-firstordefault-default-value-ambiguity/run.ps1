$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project "$root/starter/Starter.csproj"

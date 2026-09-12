$ErrorActionPreference = 'Stop'
dotnet restore "$PSScriptRoot/starter/Starter.csproj"
dotnet run --project "$PSScriptRoot/starter/Starter.csproj" -- observe

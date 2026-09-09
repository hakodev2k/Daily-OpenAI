$ErrorActionPreference = 'Stop'
dotnet test "$PSScriptRoot/starter/Invoice.Tests.csproj" --nologo

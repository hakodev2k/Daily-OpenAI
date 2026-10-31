$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet test './starter/RailYard.Access.Tests/RailYard.Access.Tests.csproj' -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Tests failed.' }
}
finally { Pop-Location }

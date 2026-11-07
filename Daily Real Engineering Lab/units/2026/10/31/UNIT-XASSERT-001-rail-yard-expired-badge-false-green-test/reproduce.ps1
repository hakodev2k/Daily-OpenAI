$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    Remove-Item Env:LAB_FAULT_INJECTION -ErrorAction SilentlyContinue
    dotnet test './starter/RailYard.Access.Tests/RailYard.Access.Tests.csproj' -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Expected starter test suite to be green.' }
    dotnet run --project './starter/AcceptanceProbe/AcceptanceProbe.csproj' -c Release -- reproduce
    if ($LASTEXITCODE -ne 0) { throw 'Independent probe did not reproduce expected starter failure.' }
    Write-Host 'REPRODUCE PASS: green tests coexist with invalid access behavior.'
}
finally {
    Pop-Location
}

$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    foreach ($project in @('starter/ProxyLab.Starter.csproj', 'probe/ProxyLab.Probe.csproj', 'solution/ProxyLab.Reference.csproj')) {
        & dotnet restore $project --nologo
        if ($LASTEXITCODE -ne 0) { throw "Restore failed: $project" }
    }
    Write-Host 'Setup complete (.NET 8 target, no third-party packages).'
}
finally { Pop-Location }

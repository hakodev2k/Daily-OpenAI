$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $root
try {
    dotnet build './starter/ProcessPipeLab.csproj' -c Release
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
    & dotnet './starter/bin/Release/net8.0/ProcessPipeLab.dll' '--bytes=524288'
    exit $LASTEXITCODE
}
finally { Pop-Location }

$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/RulesetLab.csproj -- reproduce
    if ($LASTEXITCODE -ne 0) { throw 'Expected starter symptom was not reproduced, or execution failed' }
}
finally { Pop-Location }

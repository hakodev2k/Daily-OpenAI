$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ApiPartialUpdateLab.csproj"
if ($LASTEXITCODE -eq 0) { throw 'Expected starter symptom was not reproduced.' }
Write-Host 'Reproduction confirmed: starter changed an omitted field.'

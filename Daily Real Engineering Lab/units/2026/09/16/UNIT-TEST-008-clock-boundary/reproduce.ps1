$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ClockBoundaryLab.csproj"
Write-Host "Expected starter symptom: output follows the machine clock instead of the two controlled observation timestamps."
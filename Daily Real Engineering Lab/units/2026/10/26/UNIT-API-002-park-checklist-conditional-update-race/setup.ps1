$ErrorActionPreference = "Stop"
foreach ($folder in @("starter", "solution")) {
    $project = Join-Path $PSScriptRoot "$folder/ChecklistLab.csproj"
    & dotnet restore $project
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    & dotnet build $project --no-restore
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

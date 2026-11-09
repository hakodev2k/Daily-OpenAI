param([string]$Project = "$PSScriptRoot/starter/SurveyImport.csproj")
$ErrorActionPreference = 'Stop'
dotnet run --project $Project
exit $LASTEXITCODE

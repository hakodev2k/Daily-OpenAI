dotnet run --project "$PSScriptRoot/starter/ClosureLab.csproj"
if ($LASTEXITCODE -eq 2) {
  Write-Host "Reproduction confirmed"
  exit 0
}
exit 1

$ErrorActionPreference = "Stop"
$output = dotnet run --project "$PSScriptRoot/starter/ApiContractLab.csproj"
$output | Write-Host
if ($output -notmatch "Before: .*EmailEnabled=True") { throw "Starter precondition was not observed." }
if ($output -notmatch "After : .*EmailEnabled=False") { throw "Intended starter symptom was not reproduced." }
Write-Host "REPRODUCED: an unrelated partial update changed EmailEnabled."

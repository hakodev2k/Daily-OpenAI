$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = dotnet run --project "$root/starter/LeaseLab.csproj"
$output | Write-Host
if ($output -notmatch 'DOUBLE_PROCESSING=True') {
    throw 'Expected starter symptom was not reproduced.'
}
Write-Host 'REPRODUCE_PASS'

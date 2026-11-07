$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/Lab.csproj"
$output | Write-Output
$required = @('EQUAL_SAME_SEQUENCE=True','EQUAL_DIFFERENT_ORDER=False','HASHSET_COUNT=1')
foreach ($line in $required) { if ($output -notcontains $line) { throw "Thiếu kết quả verify: $line" } }
Write-Output 'VERIFY=PASS'

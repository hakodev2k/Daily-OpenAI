$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "../starter/Lab.csproj"

dotnet restore $project | Out-Null
$output = dotnet run --project $project
Write-Host $output

$discounted = ($output | Select-String '^DiscountedResponse=(.+)$').Matches.Groups[1].Value
$normal = ($output | Select-String '^NormalResponse=(.+)$').Matches.Groups[1].Value
$cached = ($output | Select-String '^CachedBasePrice=(.+)$').Matches.Groups[1].Value

if ($discounted -ne "80") {
    Write-Error "FAIL: request khuyến mãi phải trả 80 nhưng nhận '$discounted'."
    exit 1
}

if ($normal -ne "100") {
    Write-Error "FAIL: request bình thường phải trả 100 nhưng nhận '$normal'."
    exit 1
}

if ($cached -ne "100") {
    Write-Error "FAIL: cached base price phải giữ 100 nhưng nhận '$cached'."
    exit 1
}

Write-Host "PASS: promotion không còn làm thay đổi giá của request khác và base cache vẫn đúng."

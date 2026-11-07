$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "../starter/Lab.csproj"

dotnet restore $project | Out-Null
$output = dotnet run --project $project
Write-Host $output

$discounted = ($output | Select-String '^DiscountedResponse=(.+)$').Matches.Groups[1].Value
$normal = ($output | Select-String '^NormalResponse=(.+)$').Matches.Groups[1].Value
$cached = ($output | Select-String '^CachedBasePrice=(.+)$').Matches.Groups[1].Value

if ($discounted -eq "80" -and $normal -eq "80" -and $cached -eq "80") {
    Write-Host "REPRODUCED: request bình thường bị ảnh hưởng bởi request khuyến mãi trước đó."
    exit 0
}

Write-Error "Không reproduce được symptom mong đợi. Discounted=$discounted Normal=$normal Cached=$cached"
exit 1

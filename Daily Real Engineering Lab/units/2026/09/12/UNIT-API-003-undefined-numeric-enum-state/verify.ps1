$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/OrderApi.csproj'
$process = Start-Process dotnet -ArgumentList @('run','--project',$project,'--urls','http://127.0.0.1:5063') -PassThru -WindowStyle Hidden
try {
    Start-Sleep -Seconds 3
    $client = [System.Net.Http.HttpClient]::new()

    $validContent = [System.Net.Http.StringContent]::new('{"status":"Shipped"}', [System.Text.Encoding]::UTF8, 'application/json')
    $validResponse = $client.PutAsync('http://127.0.0.1:5063/orders/42/status', $validContent).GetAwaiter().GetResult()
    $validBody = $validResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult()

    $invalidContent = [System.Net.Http.StringContent]::new('{"status":999}', [System.Text.Encoding]::UTF8, 'application/json')
    $invalidResponse = $client.PutAsync('http://127.0.0.1:5063/orders/42/status', $invalidContent).GetAwaiter().GetResult()
    $invalidBody = $invalidResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult()

    $validPass = ([int]$validResponse.StatusCode -eq 200 -and $validBody -match 'Shipped')
    $invalidPass = ([int]$invalidResponse.StatusCode -ge 400 -and [int]$invalidResponse.StatusCode -lt 500)

    if ($validPass -and $invalidPass) {
        Write-Host 'VERIFIED: documented string value succeeds and undefined numeric value is rejected.'
        exit 0
    }

    Write-Error "Verification failed. valid=$([int]$validResponse.StatusCode) $validBody invalid=$([int]$invalidResponse.StatusCode) $invalidBody"
    exit 1
}
finally {
    if ($process -and -not $process.HasExited) { Stop-Process -Id $process.Id -Force }
}
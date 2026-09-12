$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/OrderApi.csproj'
$process = Start-Process dotnet -ArgumentList @('run','--project',$project,'--urls','http://127.0.0.1:5063') -PassThru -WindowStyle Hidden
try {
    Start-Sleep -Seconds 3
    $client = [System.Net.Http.HttpClient]::new()
    $content = [System.Net.Http.StringContent]::new('{"status":999}', [System.Text.Encoding]::UTF8, 'application/json')
    $response = $client.PutAsync('http://127.0.0.1:5063/orders/42/status', $content).GetAwaiter().GetResult()
    $body = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()

    if ([int]$response.StatusCode -eq 200 -and $body -match '999') {
        Write-Host 'REPRODUCED: API accepted an out-of-contract status value.'
        exit 0
    }

    Write-Error "Expected the starter defect, but received HTTP $([int]$response.StatusCode): $body"
    exit 1
}
finally {
    if ($process -and -not $process.HasExited) { Stop-Process -Id $process.Id -Force }
}
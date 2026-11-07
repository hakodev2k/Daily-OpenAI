param(
    [Parameter(Mandatory = $true)][ValidateSet('reproduce','verify')][string]$Mode,
    [int]$Port = 5077,
    [switch]$Reference
)
$ErrorActionPreference = 'Stop'
$unitRoot = Split-Path -Parent $PSScriptRoot
$process = $null
$stdout = Join-Path $env:TEMP ("proxy-lab-" + [guid]::NewGuid().ToString('N') + ".stdout.log")
$stderr = [System.IO.Path]::ChangeExtension($stdout, '.stderr.log')
Push-Location $unitRoot
try {
    $project = if ($Reference) { 'solution/ProxyLab.Reference.csproj' } else { 'starter/ProxyLab.Starter.csproj' }
    $dll = if ($Reference) { 'solution/bin/Release/net8.0/ProxyLab.Reference.dll' } else { 'starter/bin/Release/net8.0/ProxyLab.Starter.dll' }
    foreach ($item in @($project, 'probe/ProxyLab.Probe.csproj')) {
        & dotnet build $item -c Release --nologo
        if ($LASTEXITCODE -ne 0) { throw "Build failed: $item" }
    }
    $process = Start-Process -FilePath 'dotnet' -ArgumentList @($dll, '--urls', "http://127.0.0.1:$Port") -WorkingDirectory $unitRoot -PassThru -RedirectStandardOutput $stdout -RedirectStandardError $stderr
    $ready = $false
    for ($i = 0; $i -lt 80; $i++) {
        if ($process.HasExited) { throw "Server exited before listening. See $stderr" }
        $tcp = New-Object System.Net.Sockets.TcpClient
        try {
            $tcp.Connect('127.0.0.1', $Port)
            $ready = $true
            break
        }
        catch { Start-Sleep -Milliseconds 250 }
        finally { $tcp.Dispose() }
    }
    if (-not $ready) { throw "Server not listening on port $Port. See $stderr" }
    & dotnet probe/bin/Release/net8.0/ProxyLab.Probe.dll --port $Port --mode $Mode
    if ($LASTEXITCODE -ne 0) { throw "Probe failed with exit code $LASTEXITCODE" }
}
finally {
    if ($null -ne $process -and -not $process.HasExited) {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    }
    Write-Host "Server logs: $stdout ; $stderr"
    Pop-Location
}

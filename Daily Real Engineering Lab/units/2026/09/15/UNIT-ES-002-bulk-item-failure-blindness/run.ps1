$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Copy-Item "$root/starter/bulk-response.json" "$root/starter/bin/Debug/net8.0/bulk-response.json" -Force -ErrorAction SilentlyContinue
dotnet run --project "$root/starter/BulkInspector.csproj"

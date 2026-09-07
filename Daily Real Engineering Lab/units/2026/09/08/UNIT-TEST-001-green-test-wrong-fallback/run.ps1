$ErrorActionPreference = "Stop"
$root = Join-Path $PSScriptRoot "starter"
$tests = Join-Path $root "Shipping.Tests/Shipping.Tests.csproj"
$probe = Join-Path $root "Shipping.Probe/Shipping.Probe.csproj"

dotnet restore $tests
dotnet test $tests --no-restore
dotnet run --project $probe

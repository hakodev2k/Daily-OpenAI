$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Portal.csproj'
dotnet build $project --nologo | Out-Host
$cases = @(
  @{ Base=''; Expected='/reports/42' },
  @{ Base='/ops'; Expected='/ops/reports/42' },
  @{ Base='/internal/tools'; Expected='/internal/tools/reports/42' }
)
foreach ($case in $cases) {
  $actual = dotnet run --no-build --project $project -- $case.Base
  if ($actual -ne $case.Expected) { throw "Expected '$($case.Expected)' but got '$actual' for PathBase '$($case.Base)'." }
}
Write-Host 'PASS: learner-editable starter preserves the hosting path in all cases.'
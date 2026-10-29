# Before — baseline
- `.\reproduce.ps1` PASS với `REPRODUCED`.
- Timeline: `START dispatch → STOP dispatch → START gateway-send → STOP gateway-send`.
- `Business=True`, `TraceRelation=True`, `CorrectLifetime=False`, `OriginalSymptom=True`.
- `.\verify.ps1` FAIL trên starter chưa sửa.
- Nếu không build được, kiểm tra `dotnet --list-sdks` và .NET 8 SDK.

# Before — starter chưa sửa
- `./reproduce.ps1` exit **0** và in `REPRODUCED`.
- `accepted=True`, `receipt=PAL-000`, `auditCount=1`.
- `auditInsideGate=True`: audit chạy trên đường đi đang giữ gate của producer.
- `producerElapsedMs` thường gần hoặc lớn hơn độ trễ mô phỏng 60 ms; không assert một con số tuyệt đối.
- `./verify.ps1` exit **1** và báo failure cho starter.

Nếu baseline không tái hiện: xác nhận .NET 8, source chưa sửa và không có custom SynchronizationContext. Kiểm tra `dotnet --info`.

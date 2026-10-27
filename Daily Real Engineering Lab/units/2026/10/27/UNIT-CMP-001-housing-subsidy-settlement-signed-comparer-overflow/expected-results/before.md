# Before — expected starter evidence (static review, not executed)

- ordinary: PASS, expected ORD-01,ORD-02,ORD-03.
- boundary: FAIL, expected NEG,POS nhưng actual POS,NEG.
- tie: FAIL, expected A-1,Z-9 nhưng actual Z-9,A-1.
- ./reproduce.ps1 chạy fixture boundary và báo REPRODUCED nếu starter trả exit code 2.
- ./verify.ps1 trên starter nguyên bản phải báo VERIFY FAILED.

Nếu kết quả khác, kiểm tra .NET SDK, chắc chắn chưa sửa starter và chạy dotnet run --project starter/SettlementOrderLab.csproj -- --scenario all. Đây là kết quả dự kiến từ static review, không phải log thực thi đã đo.

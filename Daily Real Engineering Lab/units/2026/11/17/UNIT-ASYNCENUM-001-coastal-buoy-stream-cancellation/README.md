# UNIT-ASYNCENUM-001 — Telemetry tiếp tục sau khi client hủy

## Mục tiêu
Điều tra cancellation contract của C# async stream, thu thập evidence và xác nhận fix qua regression.

## Bối cảnh thực tế
Gateway phao đo ven biển gửi telemetry cho dashboard qua `IAsyncEnumerable`. Dashboard đã ngắt kết nối nhưng gateway vẫn có dấu hiệu phát mẫu.

## Bạn cần làm gì
Reproduce, ghi ít nhất hai hypothesis, sửa `starter/Program.cs` và chạy verify. Không sửa assertion.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell; không cần database hay dịch vụ cloud.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
./scripts/verify.ps1
```

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` trên starter nguyên bản.

## Những gì cần quan sát
Ghi dòng `evidence cancelled=... healthy=...`, compiler warnings và các mẫu sau khi client hủy. Luồng hủy phải không phát mẫu; luồng bình thường phải phát ba mẫu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference](solution/README.md)

## Expected Results
Starter: reproduce thành công, verify thất bại. Sau sửa: verify PASS.

## Estimated Time
40 phút. Ghi nhận tại [investigation](workspace/my-investigation.md).

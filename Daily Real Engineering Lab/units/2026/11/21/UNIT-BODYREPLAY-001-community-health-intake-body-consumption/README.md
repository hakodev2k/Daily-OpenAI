# UNIT-BODYREPLAY-001 — Hồ sơ tiếp nhận biến mất sau lớp audit

## Mục tiêu
Điều tra dữ liệu request body trong ASP.NET Core middleware pipeline.

## Bối cảnh thực tế
Hệ thống tiếp nhận hồ sơ sức khỏe cộng đồng vừa bổ sung audit middleware. Audit ghi nhận payload hợp lệ nhưng handler phía sau nhận chuỗi rỗng. Vấn đề xuất hiện ở cả hồ sơ tiếng Việt.

## Bạn cần làm gì
Reproduce, ghi ít nhất hai hypothesis, phân tích evidence, sửa starter, verify, sau đó đối chiếu reference.

## Yêu cầu môi trường
.NET SDK 8, PowerShell, không cần mạng hay database.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`; script thành công khi xác nhận triệu chứng. Sửa `starter/Program.cs`, rồi chạy `./scripts/verify.ps1`.

## Những gì cần quan sát
So sánh `auditChars`, `expected`, `received` trên ba request; kiểm tra thứ tự các thành phần pipeline.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md)

## Expected Results
Starter: có mismatch; sau sửa: `VERIFY PASS` và exit code 0.

## Estimated Time
45–60 phút.

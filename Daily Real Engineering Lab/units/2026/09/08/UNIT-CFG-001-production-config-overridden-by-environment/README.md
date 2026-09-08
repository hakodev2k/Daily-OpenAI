# UNIT-CFG-001 — Production vẫn gọi staging dù appsettings đã đúng

## Mục tiêu
Điều tra một lỗi configuration/deployment trong .NET: file cấu hình production chứa endpoint đúng nhưng ứng dụng runtime vẫn gọi endpoint staging.

## Bối cảnh thực tế
Sau một release, team xác nhận source control và `appsettings.Production.json` đều trỏ đến Payments production. Tuy nhiên log startup cho thấy service vẫn dùng staging URL. Không có exception, health check vẫn xanh và deployment pipeline báo thành công.

Lab tập trung vào **configuration provider precedence**, runtime evidence và cách phân biệt “file đúng” với “effective configuration đúng”.

## Nhiệm vụ
1. Chạy starter và reproduce symptom.
2. Ghi ít nhất 2 hypotheses trước khi sửa.
3. Dùng output `GetDebugView()` để xác định provider nào thắng cho `Payments:BaseUrl`.
4. Xác định deployment state nào đang ghi đè giá trị versioned configuration.
5. Sửa nguyên nhân trong `starter/` mà không hard-code workaround trong business code.
6. Chạy `verify.ps1`.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 5.1+ hoặc PowerShell 7+

## Chạy
```powershell
./run.ps1
```

## Reproduce
```powershell
./reproduce.ps1
```

## Evidence cần thu thập
- Giá trị mong đợi từ versioned configuration.
- Giá trị effective tại runtime.
- Thứ tự các configuration providers.
- Provider cụ thể cung cấp giá trị thắng.
- Deployment input nào tạo ra override đó.

## Invariant cần bảo vệ
Production deployment không được âm thầm sử dụng endpoint của environment khác. Effective configuration phải được xác minh tại startup đối với các dependency quan trọng.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
> Spoiler: chỉ xem sau khi đã tự điều tra.
- [Reference Solution](solution/README.md)

## Expected Results
**Before:** runtime chọn `https://payments.staging.example/` dù default production là `https://payments.prod.example/`.

**After:** runtime chọn production endpoint và verification xác nhận không còn stale environment override.

## Estimated Time
30–45 phút.

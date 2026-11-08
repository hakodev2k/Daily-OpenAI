# UNIT-UNICODEKEY-001 — Hai mã hội viên trông giống nhau nhưng được cấp hai tài khoản

## Mục tiêu
Điều tra một incident integrity ở ranh giới nhập liệu, dùng evidence để xác định vì sao hệ thống chấp nhận hai định danh không nên đồng tồn tại.

## Bối cảnh thực tế
Ứng dụng đăng ký hội viên makerspace nhận tên định danh từ web và mobile. Nhân viên thấy hai thẻ hội viên trông giống hệt nhau nhưng có hai hồ sơ riêng, dẫn tới phân bổ lượt sử dụng máy hai lần.

## Bạn cần làm gì
Chạy mô phỏng, ghi ít nhất hai giả thuyết, kiểm tra độ dài và mã ký tự của đầu vào, sửa `starter/Program.cs` và chứng minh không làm hỏng các định danh thực sự khác nhau.

## Yêu cầu môi trường
PowerShell 5.1+ và .NET SDK 8; không cần network, Docker hay package ngoài.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Script reproduce chạy luồng đăng ký hội viên trên dữ liệu cố định, xác nhận lỗi đang tồn tại (exit 0 khi triệu chứng xuất hiện).

## Những gì cần quan sát
Đối chiếu mã Unicode, byte length, hiển thị, số hồ sơ và kết quả kiểm tra trùng. Ghi evidence trước khi thay đổi code. Không suy đoán nguyên nhân chỉ từ giao diện.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference solution](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
50 phút.

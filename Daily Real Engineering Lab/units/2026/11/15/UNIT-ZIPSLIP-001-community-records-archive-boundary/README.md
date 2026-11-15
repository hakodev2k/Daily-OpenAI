# UNIT-ZIPSLIP-001 — Ranh giới ghi file khi nhập kho lưu trữ cộng đồng

## Mục tiêu
Điều tra và sửa lỗi toàn vẹn file khi một service .NET nhận gói ZIP do đối tác gửi.

## Bối cảnh thực tế
Trung tâm lưu trữ cộng đồng nhập các tài liệu số hóa từ đối tác. Sau một lần nhập, file nằm ngoài thư mục đích bị thay đổi dù job báo thành công. Chỉ chạy fixture mô phỏng trong sandbox của lab.

## Bạn cần làm gì
Chạy baseline, ghi bằng chứng đường dẫn thực tế, nêu ít nhất hai giả thuyết, sửa starter và kiểm chứng cả file hợp lệ lẫn file bất thường.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell 7 hoặc Windows PowerShell 5.1. Không cần dịch vụ cloud, database hay NuGet ngoài SDK.

## Chạy nhanh
```powershell
./reproduce.ps1
./verify.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`; script tạo fixture ZIP trong `sandbox/`, chạy chương trình starter và kiểm tra triệu chứng. Dùng `./reset.ps1` để dọn.

## Những gì cần quan sát
Quan sát danh sách entry, đường dẫn ghi thực tế, nội dung file ngoài vùng extract và log console. Đừng chỉ tin exit code.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/Program.cs`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
45–60 phút.

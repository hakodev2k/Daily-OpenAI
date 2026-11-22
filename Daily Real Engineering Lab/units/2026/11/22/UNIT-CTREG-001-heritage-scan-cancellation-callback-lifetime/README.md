# UNIT-CTREG-001 — Callback hủy xuất hiện sau khi scan đã hoàn tất

## Mục tiêu
Điều tra vòng đời callback trong tác vụ .NET và thiết kế regression test phân biệt công việc đã kết thúc với công việc đang xử lý.

## Bối cảnh thực tế
Một worker số hóa hồ sơ di sản nhận lệnh shutdown sau khi đã xác nhận nhiều scan thành công. Audit bỗng ghi nhận một số scan đã hoàn tất là bị hủy. Số liệu vận hành không khớp với biên nhận đã phát.

## Bạn cần làm gì
Chạy tình huống, ghi lại thời điểm audit phát sinh, đưa ra tối thiểu hai giả thuyết, sửa mã trong `starter/Program.cs`, sau đó chứng minh bằng hai kịch bản hồi quy.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell. Không cần cloud, database hoặc package NuGet ngoài SDK.

## Chạy nhanh
Từ thư mục unit: `./scripts/setup.ps1`, `./scripts/reproduce.ps1`, sau đó `./scripts/verify.ps1`.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` trên starter chưa sửa. Lệnh thành công khi tái hiện được triệu chứng; nó không chứng nhận fix.

## Những gì cần quan sát
So sánh số audit trước và sau shutdown; kiểm tra việc một scan đã có biên nhận có thể bị ghi thêm trạng thái khác hay không. Quan sát riêng tình huống shutdown trong khi scan đang chạy.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md).

## Expected Results
[Trước](expected-results/before.md) · [Sau](expected-results/after.md). `verify.ps1` phải thất bại trên starter gốc và thành công sau khi sửa đúng.

## Estimated Time
35–45 phút.

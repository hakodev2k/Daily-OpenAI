# UNIT-LINQSNAP-001 — Manifest hàng hóa thay đổi sau khi chốt ca

## Mục tiêu
Điều tra sai lệch giữa dữ liệu được xác nhận tại thời điểm chốt ca và bản manifest được gửi cho đối tác; phân biệt dữ liệu nguồn, thời điểm xử lý và bằng chứng xuất dữ liệu.

## Bối cảnh thực tế
Hệ thống cảng tạo manifest container cho hãng tàu. Nhân viên xác nhận danh sách lúc 17:00, nhưng lúc 17:05 một container được cập nhật trạng thái. Báo cáo xuất lúc 17:10 không còn khớp bản xác nhận; không có exception và log báo thành công.

## Bạn cần làm gì
Chạy tình huống, ghi lại 3 giả thuyết, xác định thời điểm dữ liệu đổi, đề xuất cách bảo đảm manifest bất biến sau khi xác nhận, sửa `starter/Program.cs` rồi chạy verify.

## Yêu cầu môi trường
.NET SDK 8.0, PowerShell 7 hoặc Windows PowerShell; không cần database hay cloud.

## Chạy nhanh
Từ thư mục unit: `./scripts/reproduce.ps1`. Nếu dùng Windows PowerShell: `powershell -ExecutionPolicy Bypass -File ./scripts/reproduce.ps1`.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`; chương trình in EXPECTED và ACTUAL, script xác nhận sai lệch có thể lặp lại.

## Những gì cần quan sát
Đối chiếu mã container, trọng lượng và tổng trọng lượng trước/sau cập nhật. Xác định những bước có thể khiến dữ liệu hiển thị khác bản chốt, không giả định ngay nguyên nhân.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md) chỉ xem sau khi thử sửa.

## Expected Results
Trước: sai lệch manifest được phát hiện. Sau: `./scripts/verify.ps1` trả exit code 0 và báo `VERIFY PASS`; tổng cân nặng chốt là 30 tấn, bất kể bản ghi vận hành cập nhật sau đó.

## Estimated Time
45–60 phút.

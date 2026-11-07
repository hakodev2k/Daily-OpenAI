# UNIT-HASHKEY-001 — Phiếu mượn bảo tàng biến mất khỏi registry

## Mục tiêu
Điều tra một lỗi tra cứu khó thấy trong bộ nhớ .NET và thiết kế regression check cho thay đổi nghiệp vụ.

## Bối cảnh thực tế
Một dịch vụ quản lý hiện vật lưu trạng thái duyệt phiếu mượn trong bộ nhớ. Khi nhân viên sửa năm của phiếu vừa nhập, hệ thống vẫn hiển thị tổng số phiếu là 1 nhưng không tra được trạng thái đã duyệt.

## Bạn cần làm gì
Reproduce lỗi, ghi lại hai giả thuyết, phân biệt dữ liệu mất thật với lỗi truy cập, sửa starter mà không làm mất trạng thái và chứng minh cả lookup cũ/mới theo chính sách cập nhật.

## Yêu cầu môi trường
.NET SDK 8; PowerShell 7 hoặc Windows PowerShell. Không cần dịch vụ ngoài.

## Chạy nhanh
Tại thư mục unit: `dotnet run --project starter/LoanRegistry.csproj`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`; script chỉ thành công khi phát hiện đúng triệu chứng lỗi. Không sửa starter trước bước này.

## Những gì cần quan sát
So sánh lookup trước/sau thao tác chỉnh sửa, trạng thái trả về và Count. Ghi nhận exit code, xác định liệu Count chứng minh lookup vẫn hoạt động hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ đọc sau khi thử sửa.

## Expected Results
Trước: lookup sau sửa trả false dù Count=1. Sau: lookup theo năm mới thành công, trạng thái APPROVED, Count=1 và khóa cũ không còn hợp lệ. Nếu reproduce không như dự kiến, kiểm tra SDK và chạy lại từ bản starter gốc.

## Estimated Time
35–45 phút (L1 / D4).

# UNIT-JSONDUP-001 — Hai service, một JSON: quyết định phê duyệt mâu thuẫn

## Mục tiêu
Điều tra sự khác biệt giữa bằng chứng audit và kết quả xử lý, đề xuất ranh giới kiểm soát đầu vào và xác minh bằng regression tests.

## Bối cảnh thực tế
Nền tảng cấp hỗ trợ thiên tai tiếp nhận quyết định phê duyệt từ một gateway. Audit hiển thị **từ chối**, trong khi worker cấp tiền lại hiển thị **chấp thuận** cho cùng một payload. Không có lỗi HTTP hay exception. Đội vận hành phải xác định liệu đây là vấn đề logging, serialization, mapping hay hợp đồng dữ liệu.

## Bạn cần làm gì
Chạy replay; ghi lại hai cách diễn giải; lập ít nhất hai giả thuyết; sửa mã ở starter để một payload mơ hồ không thể được xử lý thành hai quyết định khác nhau; chứng minh payload hợp lệ vẫn được chấp nhận.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell 7 hoặc Windows PowerShell; không cần Azure, Docker hay NuGet ngoài SDK.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
./verify.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` và so sánh `AUDIT` với `PAYOUT`. Đầu vào nằm trong `starter/Program.cs`. Không sửa dữ liệu mẫu trước lần reproduce đầu tiên.

## Những gì cần quan sát
Một sự kiện có cùng mã giao dịch nhưng hai quyết định trái ngược. Ghi lại raw payload, kết quả của từng consumer và lý do test kiểm soát đầu vào thất bại.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `investigation.md`.
3. Thử fix trong `starter/Program.cs`.
4. Chạy `./verify.ps1` đến khi PASS.
5. Chỉ sau đó mới xem solution.

## Hints
[Xem gợi ý theo cấp](hints.md)

## Reference Solution
**Spoiler:** [Xem giải pháp tham khảo](solution/README.md).

## Expected Results
Reproduce hiển thị hai quyết định khác nhau. Sau khi sửa, mọi payload có cấu trúc mơ hồ phải bị từ chối trước khi consumer xử lý; payload hợp lệ vẫn hoạt động.

## Estimated Time
45 phút.

# UNIT-EFBULK-001 — Museum Ticket Price: Bulk Update and Tracked State

## Mục tiêu
Điều tra một sai lệch giữa kết quả cập nhật hàng loạt và dữ liệu hiển thị trong cùng request; sửa có kiểm chứng và bảo toàn các thay đổi chưa lưu của entity khác.

## Bối cảnh thực tế
Hệ thống bán vé bảo tàng cho phép nhân viên áp dụng phụ phí ngày lễ lên một nhóm vé. API báo đã cập nhật một dòng nhưng màn hình xác nhận vẫn hiển thị giá cũ. Một request khác đọc database lại thấy giá mới.

## Bạn cần làm gì
1. Chạy tình huống và ghi nhận giá trước/sau, số dòng cập nhật, dữ liệu đọc lại.
2. Đưa ra ít nhất hai hypothesis; xác định thành phần nào giữ phiên bản dữ liệu khác nhau.
3. Sửa trong `starter/TicketPricingService.cs` để response phản ánh giá đã commit, đồng thời giữ nguyên pending edit của vé khác.
4. Chạy verification, giải thích trade-off và giới hạn của cách sửa.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell, internet lần đầu để restore NuGet; EF Core Sqlite 8.0.11.

## Chạy nhanh
Từ thư mục unit:
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` chạy bản starter gốc qua một harness độc lập, yêu cầu tái hiện sự khác biệt và trả exit code 0 khi symptom xuất hiện. Không dùng script này để chứng nhận fix.

## Những gì cần quan sát
Số row cập nhật, giá phản hồi, giá đọc bằng context mới và trạng thái một vé khác đang có thay đổi chưa lưu. Chú ý các quan sát phải được ghi trước khi mở solution.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference](solution/README.md) — chỉ xem sau khi tự verify.

## Expected Results
Starter: số row cập nhật là 1, nhưng response chứa giá cũ; context mới thấy giá mới. Fixed: response đúng giá mới, database đúng, pending edit độc lập vẫn được giữ, không có update lặp.

## Estimated Time
60–90 phút (L4 / D4).
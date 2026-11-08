# UNIT-SEARCHCUT-001 — Chuyển search index không gián đoạn khi dữ liệu vẫn thay đổi

## Mục tiêu
Thiết kế và bảo vệ quyết định migration cho hệ thống tìm kiếm đang phục vụ traffic thật; phân biệt chỉ số sẵn sàng kỹ thuật với tính đầy đủ dữ liệu, và lập kế hoạch rollback có thể kiểm chứng.

## Bối cảnh thực tế
Cổng tra cứu hồ sơ công khai của thành phố có 2,4 triệu bản ghi; 40 request tìm kiếm/giây ở giờ cao điểm; 120 thay đổi hồ sơ/phút. Đợt cập nhật analyzer bắt buộc rebuild. Trong buổi diễn tập, người dân báo hồ sơ mới chỉnh sửa không còn xuất hiện và số kết quả dao động. Search là read model; PostgreSQL vẫn là nguồn sự thật.

## Bạn cần làm gì
1. Đọc [evidence/incident-timeline.md](evidence/incident-timeline.md), phân loại dữ kiện và giả thuyết.
2. Điền [workspace/decision-record.md](workspace/decision-record.md): ít nhất 3 phương án, tiêu chí chọn, cutover/rollback, test plan.
3. Chạy diễn tập trên giấy theo [evidence/change-stream.csv](evidence/change-stream.csv): tính trạng thái index theo từng mốc và chỉ ra dữ liệu nào chưa thể công bố.
4. So sánh [verification/acceptance-matrix.md](verification/acceptance-matrix.md) và tự đánh giá từng tiêu chí.
5. Chỉ sau khi hoàn thành mới mở reference solution.

## Yêu cầu môi trường
Không cần cloud hoặc dịch vụ trả phí. Chỉ cần Markdown editor, spreadsheet/CSV reader; PowerShell 7 tùy chọn để đọc dữ liệu.

## Chạy nhanh
Tại thư mục unit:
```powershell
Get-Content evidence/incident-timeline.md
Import-Csv evidence/change-stream.csv | Format-Table
Copy-Item workspace/decision-record.md workspace/my-decision.md
```

## Cách reproduce vấn đề
Mô phỏng timeline: tại 09:00 bắt đầu rebuild, tại 09:03 có cập nhật mới, tại 09:06 đổi read target; đối chiếu số lượng và revision theo từng index trong evidence. Xác định ít nhất một tình huống người dùng nhận dữ liệu cũ hoặc thiếu; không cần triển khai production.

## Những gì cần quan sát
Quan sát sự khác biệt giữa document count, high-water mark, dữ liệu sửa đổi sau snapshot, query availability và khả năng quay lại index cũ. Đừng suy diễn từ health=green rằng mọi thay đổi đã có mặt.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** [Một phương án có thể bảo vệ được](solution/reference.md).

## Expected Results
Trước: tồn tại cửa sổ đọc thiếu bản ghi hoặc bỏ lỡ cập nhật. Sau: kế hoạch chứng minh không mất update, read availability và rollback có điều kiện; tiêu chí đánh giá ở verification.

## Estimated Time
75 phút.

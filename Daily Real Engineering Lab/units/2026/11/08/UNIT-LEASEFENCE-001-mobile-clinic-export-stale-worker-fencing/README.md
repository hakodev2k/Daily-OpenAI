# UNIT-LEASEFENCE-001 — Báo cáo phòng khám bị ghi đè sau khi worker mất lease

## Mục tiêu
Điều tra sự cố nhiều worker cùng xử lý một báo cáo xuất dữ liệu; thiết kế ranh giới ghi kết quả an toàn khi timeout, retry và worker cũ hồi phục.

## Bối cảnh thực tế
Hệ thống phòng khám lưu động xuất bản tổng hợp lịch khám hằng ngày. Job chạy trên hai worker; mỗi job có một lease với thời hạn 30 giây. Sau một lần mất kết nối, file đã được đánh dấu hoàn tất lại chứa dữ liệu của bản chạy cũ.

## Bạn cần làm gì
1. Dùng bảng sự kiện trong `evidence/timeline.csv` để xác định các thời điểm lease chuyển chủ.
2. Đọc `docs/scenario.md`, tái hiện bằng bảng replay và chỉ ra các ghi dữ liệu có thể chấp nhận.
3. Đề xuất giao thức ownership, commit, retry và recovery; kiểm tra bằng `docs/acceptance-cases.md`.
4. Ghi lại ít nhất hai phương án, lý do chọn và cách giám sát production.

## Yêu cầu môi trường
Không cần cloud, database hoặc secrets. Chỉ cần Markdown editor; tùy chọn PowerShell 7 để xem dữ liệu CSV.

## Chạy nhanh
Mở `evidence/timeline.csv` và điền `workspace/my-investigation.md`. Trên PowerShell: `Import-Csv ./evidence/timeline.csv | Format-Table`.

## Cách reproduce vấn đề
Replay các bước theo thứ tự timestamp; tại mỗi sự kiện WRITE, mô phỏng chính sách hiện tại được mô tả trong scenario. So sánh phiên bản cuối cùng của report với bản mong đợi.

## Những gì cần quan sát
Theo dõi thời điểm cấp lease, timeout, worker phục hồi, lần ghi cuối cùng, và liệu status `COMPLETED` có đồng nghĩa output mới nhất hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Phân tích và thiết kế tham chiếu](solution/README.md) — chỉ mở sau khi hoàn tất replay.

## Expected Results
Bản xuất cuối không bị ghi đè bởi kết quả của worker không còn quyền; retry và crash được xử lý có thể kiểm chứng.

## Estimated Time
55 phút.
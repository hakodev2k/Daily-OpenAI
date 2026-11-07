# UNIT-SEARCH-001 — Cắt chuyển search index v2 mà không làm gián đoạn người thuê nhà

## Mục tiêu

Thiết kế và bảo vệ một kế hoạch thay đổi search schema trên hệ thống đang phục vụ traffic thật. Tự ước lượng thời gian, phân tích evidence, đề xuất rollout/cutover/rollback và viết một ADR có thể đưa vào production review. Lab thuộc **L5 Design Decision / D4**, không phải một bài tìm một dòng code sai.

## Bối cảnh thực tế

Nền tảng tìm phòng thuê ngắn hạn có 18 triệu listing đang hoạt động. Search index hiện tại cần hỗ trợ bộ lọc tiện nghi đã chuẩn hóa và tọa độ tìm kiếm chính xác hơn. Đội vận hành từng thử cập nhật index ngay trên đường phục vụ traffic và gặp hiện tượng kết quả không ổn định, p95 tăng và khách không tìm thấy phòng vẫn còn mở bán. Product yêu cầu thay đổi schema mà không ngừng tìm kiếm.

## Bạn cần làm gì

1. Đọc [yêu cầu và ràng buộc](starter/requirements.md), [timeline](evidence/incident-timeline.md) và [bảng năng lực](evidence/capacity.csv).
2. Chạy mô hình năng lực, phân biệt throughput backfill, nguồn thay đổi và tốc độ catch-up.
3. Ghi ít nhất ba phương án, chọn một phương án có lý do, kèm điều kiện không được cutover.
4. Hoàn thành [ADR của bạn](workspace/my-adr.md): data flow, consistency, security, capacity, validation, rollout, rollback, owner và observability.
5. Đối chiếu với [acceptance gates](expected-results/after.md) trước khi mở reference.

## Yêu cầu môi trường

PowerShell 5.1+ hoặc PowerShell 7+ để chạy capacity model; không cần cloud account, Docker, Elasticsearch hay dịch vụ trả phí. Mọi số liệu đều là synthetic và đã cung cấp trong repository.

## Chạy nhanh

Từ thư mục unit:

```powershell
pwsh -NoProfile -File .\run.ps1
# Hoặc trên Windows PowerShell 5.1:
powershell -NoProfile -File .\run.ps1
```

Sau đó mở `workspace/my-adr.md` và tự điền phương án.

## Cách reproduce vấn đề

1. Đọc [timeline sự cố giả lập](evidence/incident-timeline.md) để tái dựng các mốc quan sát được.
2. Chạy `run.ps1`; ghi kết quả baseline và hai trường hợp tốc độ cập nhật tăng hoặc replay chậm.
3. Dựa trên số liệu, mô tả điều gì có thể xảy ra nếu cutover khi index mới chưa đáp ứng các acceptance gates. Đây là design exercise; không có lệnh gây lỗi lên search cluster thật.

## Những gì cần quan sát

- Search availability, p95 latency, freshness lag, tỷ lệ missing listing và thời gian tồn tại của thay đổi.
- Sự khác biệt giữa tổng số tài liệu và độ đúng của kết quả.
- Tốc độ dữ liệu mới xuất hiện so với tốc độ xử lý hàng đợi.
- Điều kiện rollback vẫn khả thi sau khi ứng dụng chuyển sang schema mới.

## Quy tắc làm lab

1. Reproduce bằng evidence và mô hình trước.
2. Ghi hypothesis và các giả định cần xác nhận.
3. Tự thiết kế phương án và risk register.
4. Verify qua acceptance gates và các tình huống thay đổi tải.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ mở sau khi tự hoàn thành ADR:** [Một phương án có thể bảo vệ được](solution/README.md).

## Expected Results

- [Trước khi thiết kế](expected-results/before.md)
- [Sau khi thiết kế: acceptance gates](expected-results/after.md)

## Estimated Time

70–90 phút. Phần khó nhất là chứng minh cutover và rollback an toàn, không phải viết sơ đồ đẹp.

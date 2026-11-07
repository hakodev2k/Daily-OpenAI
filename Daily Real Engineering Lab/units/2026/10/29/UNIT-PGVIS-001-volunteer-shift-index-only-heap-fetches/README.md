# UNIT-PGVIS-001 — Báo cáo ca tình nguyện chậm dù có covering index

## Mục tiêu
Điều tra truy vấn PostgreSQL đã có covering index nhưng số lần đọc heap tăng sau một tác vụ cập nhật. Thu thập bằng chứng, thử biện pháp khắc phục, đánh giá vận hành.

## Bối cảnh thực tế
Hệ thống điều phối tình nguyện viên xuất danh sách người đã xác nhận ca. Sau khi tác vụ đồng bộ trạng thái hoạt động chạy, báo cáo đọc chậm hơn dù truy vấn và index không đổi. Một dashboard nghi ngờ CPU, nhưng chưa có bằng chứng.

## Bạn cần làm gì
Chạy fixture, ghi execution plan và số lần đọc heap, phân biệt triệu chứng với nguyên nhân, sửa `workspace/remediation.sql`, verify và đề xuất cách tránh tái phát.

## Yêu cầu môi trường
PowerShell 7+ và Docker Desktop đang chạy. Image cố định `postgres:16.6`; không cần Azure hay dịch vụ trả phí. Container `relab-pgvis-20261029` dùng credential local `lab/labpass`, tuyệt đối không dùng trong production.

## Chạy nhanh
Từ thư mục unit:
```powershell
./setup.ps1
./reproduce.ps1
./run.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` tái tạo dữ liệu, chạy job cập nhật và xác nhận `Heap Fetches > 0`. Đây là kiểm tra trạng thái lỗi, **không** kiểm tra bản sửa.

## Những gì cần quan sát
- Query lọc ngày/trạng thái trả đúng 5.000 ID.
- `EXPLAIN (ANALYZE, BUFFERS, FORMAT JSON)`: node và `Heap Fetches` trước/sau cập nhật.
- Độ trễ ms thay đổi theo máy, không dùng ngưỡng cố định.
- Ghi tối thiểu hai giả thuyết và một bằng chứng loại trừ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [my-investigation.md](workspace/my-investigation.md).
3. Tự sửa [remediation.sql](workspace/remediation.sql).
4. Chạy `./verify.ps1` để kiểm tra chính file của bạn.
5. Sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) chỉ đọc sau khi thử sửa.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
65 phút. [Incident evidence & investigation](docs/incident.md).

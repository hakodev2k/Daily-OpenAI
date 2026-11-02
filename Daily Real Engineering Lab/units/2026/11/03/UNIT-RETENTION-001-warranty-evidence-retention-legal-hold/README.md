# UNIT-RETENTION-001 — Bằng chứng bảo hành: retention, legal hold và xóa dữ liệu xuyên hai kho

## Mục tiêu
Ra quyết định kỹ thuật về vòng đời bằng chứng số trải trên PostgreSQL và object storage; cân bằng retention, ngoại lệ legal hold, audit, khả năng phục hồi và chi phí vận hành.

## Bối cảnh thực tế
Nền tảng bảo hành thiết bị gia dụng ghi metadata hồ sơ trong PostgreSQL, ảnh/video trong object storage. Một đợt dọn dữ liệu khiến một số hồ sơ quá hạn vẫn còn file, trong khi một số hồ sơ tranh chấp lại thiếu bằng chứng. Nhóm pháp chế yêu cầu một thiết kế có thể kiểm chứng.

## Bạn cần làm gì
1. Đọc [ràng buộc](docs/scenario.md), replay [timeline](evidence/incident-timeline.md) và [dataset](evidence/cases.csv).
2. Tại snapshot 03:13 ngày 03/11/2026 (+07), lập decision table, phân loại từng hồ sơ và ghi giả thuyết vào [workspace](workspace/my-decision.md).
3. So sánh storage lifecycle, cron theo DB và phương án có state/reconciliation; bảo vệ lựa chọn bằng ADR.
4. Xử lý trường hợp hold đến trong khi delete đang được xử lý, timeout, retry, crash, backup restore.
5. Kiểm chứng bằng [12 acceptance checks](docs/acceptance-checks.md).

## Yêu cầu môi trường
Git và Markdown editor; không cần .NET, Docker, Azure hay dịch vụ trả phí. Đây là L5 Design Decision Lab dùng replay evidence, không phải executable bug lab.

## Chạy nhanh
```powershell
git pull
# Mở docs/scenario.md, evidence/cases.csv, evidence/incident-timeline.md
# Điền workspace/my-decision.md và chạy mental replay theo docs/acceptance-checks.md
```

## Cách reproduce vấn đề
Với từng claim trong CSV, tính mốc 30 ngày sau `closedAt`, đối chiếu hold và trạng thái hai kho ở 03:13 (+07). Xác định tối thiểu ba bất nhất hoặc rủi ro, kèm bằng chứng. Đây là replay incident mô phỏng, nên không có `reproduce.ps1`.

## Những gì cần quan sát
Phân biệt thời điểm được phép xóa với deadline hoàn tất; phân biệt trạng thái metadata với tình trạng object thực tế. Không kết luận chỉ dựa vào HTTP 2xx.

## Quy tắc làm lab
1. Replay trước.
2. Ghi hypothesis và bằng chứng.
3. Đề xuất giải pháp, phân tích trade-offs.
4. Verify qua acceptance checks.
5. Chỉ sau đó mới mở solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Một phương án tham khảo](solution/README.md), chỉ đọc sau khi tự hoàn thành.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
75 phút · L5 / D4. [Senior Challenge](extensions/senior-challenge.md) là phần tùy chọn.

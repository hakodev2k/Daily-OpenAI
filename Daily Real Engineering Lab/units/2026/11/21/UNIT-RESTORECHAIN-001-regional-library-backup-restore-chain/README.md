# UNIT-RESTORECHAIN-001 — Chuỗi backup có thực sự khôi phục được dữ liệu?

## Mục tiêu
Điều tra khả năng khôi phục SQL Server bằng backup evidence và đưa ra quyết định disaster recovery có thể audit.

## Bối cảnh thực tế
18 thư viện quận phục vụ 450 giao dịch/phút. Database lỗi lúc 10:17; dashboard backup vẫn báo SUCCESS. SLA: RPO ≤15 phút, RTO ≤60 phút.

## Bạn cần làm gì
Đọc evidence, dựng timeline, ghi ba hypothesis, lập restore sequence và ước lượng RPO/RTO, đánh giá hai nhánh tail-log khả thi hoặc không, viết go/no-go và acceptance checks trong workspace.

## Yêu cầu môi trường
Máy đọc Markdown/CSV, PowerShell tùy chọn. Không cần SQL Server; dữ liệu là fixture mô phỏng, không dùng trên production.

## Chạy nhanh
Mở `evidence/backup-history.csv` và `evidence/incident-timeline.md`. Có thể chạy `Import-Csv .\evidence\backup-history.csv | Format-Table` từ thư mục unit.

## Cách reproduce vấn đề
Dựng restore chain cho target 10:16:59; kiểm tra tính liên tục và khả năng đọc của từng backup. Đối chiếu incident timeline và xác định điểm cuối thực sự có chứng cứ.

## Những gì cần quan sát
Job SUCCESS khác với restore thành công; thời gian restore, integrity check và cutover ảnh hưởng RTO; cần phân biệt RPO mục tiêu với RPO chứng minh được.

## Quy tắc làm lab
1. Dựng timeline. 2. Ghi hypothesis. 3. Đề xuất recovery. 4. Kiểm tra acceptance. 5. Sau đó mới đọc solution.

## Hints
[Hint 1](hints/hint-01.md), [Hint 2](hints/hint-02.md), [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md).

## Expected Results
Một quyết định restore có chuỗi backup, giới hạn mất dữ liệu, critical path, fallback và tiêu chí mở lại dịch vụ.

## Estimated Time
75 phút.

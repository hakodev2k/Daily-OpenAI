# UNIT-PAGETOKEN-001 — Sổ quyên góp: phân trang bị lặp và bỏ sót khi dữ liệu thay đổi

## Mục tiêu
Tái hiện sai lệch dữ liệu qua nhiều request và sửa contract phân trang dựa trên evidence.

## Bối cảnh thực tế
Một tổ chức phi lợi nhuận xuất sổ quyên góp. Khi auditor duyệt các trang, giao dịch mới vẫn được ghi. Báo cáo có dòng lặp và thiếu dù từng request trả 200.

## Bạn cần làm gì
Chạy script, ghi ID từng trang, nêu hai giả thuyết, thiết kế sửa đổi và kiểm tra các trường hợp insert xen giữa và timestamp đồng hạng.

## Yêu cầu môi trường
SQL Server 2019+ hoặc Azure SQL; có thể đọc evidence để làm offline.

## Chạy nhanh
Chạy từng statement theo thứ tự trong `starter/page-replay.sql` bằng SSMS.

## Cách reproduce vấn đề
Lấy trang 1, thêm giao dịch, lấy trang 2, so sánh ID.

## Những gì cần quan sát
Số ID duy nhất, ID lặp, ID thiếu và sự thay đổi thứ tự trước/sau insert.

## Quy tắc làm lab
1. Reproduce trước. 2. Ghi hypothesis. 3. Thử fix. 4. Verify. 5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
[Cảnh báo spoiler](solution/README.md)

## Expected Results
Trước: ID lặp hoặc bỏ sót. Sau: duyệt đúng phạm vi đã định nghĩa, không lặp ID; contract xác định liệu giao dịch mới có được đưa vào không.

## Estimated Time
50 phút.

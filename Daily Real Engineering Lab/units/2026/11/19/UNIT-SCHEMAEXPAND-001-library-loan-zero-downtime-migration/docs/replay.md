# Tabletop replay và acceptance gates
Đây là **mô phỏng ra quyết định**, không phải bài test SQL chạy thực. Ghi kết quả vào workspace.
## Trạng thái
- S0: schema cũ; V1 đang ghi; Loan có 48M rows.
- S1: schema đã mở rộng; V1 vẫn ghi.
- S2: backfill đang chạy; V1/V2 cùng chạy.
- S3: backfill hoàn tất, V2 100%; rollback vẫn được yêu cầu.
- S4: sau khi xác nhận không còn V1, có thể contract.
## Failure cards
A. Backfill dừng ở 60% vì deploy worker restart.
B. V1 tạo Loan mới trong lúc backfill.
C. V2 ghi BorrowerKey mới; rollback sang V1.
D. DDL chờ schema lock 20 giây lúc cao điểm.
E. 0.2% rows không ánh xạ được BorrowerCode.
## Nhiệm vụ
1. Tạo bảng S0–S4 gồm schema, phiên bản app được phép, read/write contract và rollback action.
2. Với mỗi failure card, nêu quan sát, hành động an toàn, điều kiện retry và điều kiện dừng.
3. Định nghĩa truy vấn kiểm tra null, duplicate, count, lag, lock wait và error rate (pseudocode SQL được chấp nhận).
4. Chứng minh 2 bất biến: mọi write của V1/V2 được bảo toàn; không contract khi còn writer V1.
5. Quyết định go/no-go ở mỗi phase với ngưỡng: p95 <=250 ms, error <0.5%, backfill lag <=5 phút trước cutover, số rows chưa ánh xạ = 0 trước enforcement.
## Kiểm chứng
Pass khi cả 5 failure cards có recovery an toàn, hai bất biến được chứng minh, không có thao tác phá schema khi còn old writer, và rollback không cần phục hồi database snapshot. Nếu không, ghi FAIL và sửa phương án.

# UNIT-DRBLOB-001 — Khôi phục hồ sơ nhận nuôi sau sự cố vùng

## Mục tiêu
Thiết kế và kiểm chứng một quy trình disaster recovery (DR) bảo toàn khả năng truy cập tài liệu đính kèm, không chỉ khôi phục bản ghi SQL.

## Bối cảnh thực tế
Hệ thống nhận nuôi thú cưng của một mạng lưới shelter lưu hồ sơ trong PostgreSQL và tài liệu tiêm chủng trong object storage. Sau sự cố mất toàn bộ region chính, trang tra cứu ở region dự phòng hiển thị hồ sơ nhưng một số giấy tờ trả về 404. Báo cáo vận hành vẫn ghi RPO = 5 phút.

## Bạn cần làm gì
1. Chạy mô phỏng sự cố và ghi bằng chứng vào `workspace/my-investigation.md`.
2. Phân tích ít nhất ba giả thuyết, bao gồm khả năng khác nhau giữa các data store.
3. Đề xuất ADR: recovery boundary, publish/readiness gate, xử lý hồ sơ chưa đủ chứng từ, RPO/RTO, rollback và drill.
4. Sửa `workspace/decision.json` để mô hình hóa lựa chọn. Chạy `scripts/verify.ps1`.
5. Chỉ đọc `solution/README.md` sau khi tự đưa ra quyết định.

## Yêu cầu môi trường
PowerShell 7+; không cần cloud, database, credential hay network.

## Chạy nhanh
```powershell
pwsh -NoProfile -File ./scripts/reproduce.ps1
pwsh -NoProfile -File ./scripts/verify.ps1
```
Chạy lệnh từ thư mục unit.

## Cách reproduce vấn đề
Chạy `reproduce.ps1` trên fixture cố định. Quan sát các hồ sơ có metadata nhưng tài liệu không tải được trong cửa sổ khôi phục.

## Những gì cần quan sát
Số hồ sơ visible, số tài liệu available, mốc thời gian commit metadata, mốc object replication, và chênh lệch giữa kết quả khôi phục với SLA công bố.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix/decision.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
[Spoiler — Reference Solution](solution/README.md)

## Expected Results
Trước: ít nhất một hồ sơ hiển thị dù thiếu tài liệu bắt buộc. Sau: hồ sơ thiếu tài liệu không được đánh dấu sẵn sàng, trong khi hồ sơ đầy đủ vẫn có thể tra cứu; tất cả trường hợp được hạch toán.

## Estimated Time
50–65 phút.

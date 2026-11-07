# UNIT-REPLALAG-001 — Bản điểm đã thanh toán nhưng vẫn hiển thị bản cũ

## Mục tiêu
Đưa ra quyết định thiết kế cho một luồng đọc dữ liệu sau ghi, dựa trên timeline, số liệu độ trễ, yêu cầu sản phẩm và chi phí. Phân biệt tính đúng đắn với độ sẵn sàng của dữ liệu.

## Bối cảnh thực tế
Cổng sinh viên cấp bản điểm điện tử. Sau khi thanh toán thành công, người dùng bấm **Tải bản điểm**; một số lượt nhận 404 hoặc bản điểm chưa cập nhật dù giao dịch đã được ghi nhận. Nhóm vận hành báo cáo database không lỗi và HTTP p95 nhìn chung tốt.

## Bạn cần làm gì
1. Phân tích `evidence/timeline.csv` và `evidence/constraints.md`; tách triệu chứng, dữ kiện, giả thuyết.
2. Vẽ đường đi ghi và đọc hiện tại, xác định các điểm có thể làm sai cam kết sản phẩm.
3. So sánh tối thiểu ba phương án trong `workspace/decision.md` theo correctness, availability, latency, cost, operations.
4. Chọn một phương án; viết contract API, kế hoạch rollout, rollback, đo lường và test failure paths.
5. Đọc reference decision sau khi tự hoàn thành.

## Yêu cầu môi trường
Chỉ cần editor đọc Markdown/CSV; không cần cloud, database hoặc dịch vụ trả phí.

## Chạy nhanh
```powershell
Get-Content .\evidence\timeline.csv
Get-Content .\evidence\constraints.md
Copy-Item .\workspace\decision-template.md .\workspace\decision.md
```
Chạy trong thư mục unit. Lab là bài thiết kế có dữ liệu replay, không phải executable bug; không cần `reproduce.ps1` hay `verify.ps1`.

## Cách reproduce vấn đề
Replay từng dòng timeline theo `requestId`; đặt câu hỏi mỗi phản hồi có đáp ứng cam kết tải bản điểm ngay sau thanh toán không. Phân tích cả trường hợp chậm và trường hợp không lỗi.

## Những gì cần quan sát
So sánh thời điểm commit, thời điểm phiên bản xuất hiện ở từng đường đọc, status code và revision được trả về. Không suy luận nguyên nhân chỉ từ p95 tổng thể.

## Quy tắc làm lab
1. Reproduce trên timeline trước.
2. Ghi ít nhất ba hypotheses.
3. Đề xuất quyết định và giới hạn áp dụng.
4. Kiểm chứng bằng bảng acceptance/failure-path.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Một phương án tham chiếu và trade-offs](solution/README.md). Chỉ đọc sau khi hoàn thành decision record.

## Expected Results
- Xác định được các request không đạt contract theo revision và thời gian.
- Đề xuất chính sách nhất quán có phạm vi rõ, không buộc mọi request phải trả giá như nhau.
- Có regression matrix cho retry, replica lag, primary outage và stale revision.

## Estimated Time
65 phút.

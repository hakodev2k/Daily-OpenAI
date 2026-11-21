# UNIT-CACHEVARY-001 — Sai biến thể nội dung tại edge cache
## Mục tiêu
Tái hiện sự cố cache sai ngôn ngữ và nhóm đối tượng, thu thập evidence, đề xuất chính sách cache, kiểm tra trade-offs và yêu cầu thay đổi.
## Bối cảnh thực tế
Cổng thông báo giao thông công cộng bật CDN cache để giảm tải origin. Một số người dùng thấy nội dung ngôn ngữ/nhóm đối tượng không đúng, trong khi origin trả đúng khi gọi trực tiếp. Không có dữ liệu cá nhân trong advisory.
## Bạn cần làm gì
1. Replay tuần tự `evidence/requests.csv` theo chính sách baseline.
2. Đối chiếu `evidence/observed-cache.csv`, ghi hypothesis vào workspace.
3. Đề xuất ít nhất hai chính sách, định lượng hit ratio, số entries và correctness.
4. Đọc change request trong `evidence/requirements.md`, cập nhật thiết kế và verification.
## Yêu cầu môi trường
Chỉ cần Markdown/CSV reader, hoặc spreadsheet/PowerShell 7; không cần Azure, CDN thật hay tài khoản trả phí.
## Chạy nhanh
Khởi tạo cache rỗng. Xử lý request 1→10. Mỗi MISS lấy expected origin rồi lưu; HIT trả body đã lưu. Không có expiry hay eviction trong trace.
## Cách reproduce vấn đề
Dùng key baseline được ghi trong evidence; so sánh body trả về với expected origin. Kiểm tra số mismatch và tỷ lệ hit.
## Những gì cần quan sát
Bảng cache key, hit/miss, body, expected và sai lệch. Phân biệt edge và origin bằng evidence; chưa kết luận trước khi replay.
## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.
## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)
## Reference Solution
**Spoiler** — [Chỉ xem sau khi tự thử](solution/README.md).
## Expected Results
Baseline tái hiện response sai biến thể; phương án mới bảo đảm đúng toàn bộ trace và nêu được chi phí cache, invalidation, regression.
## Estimated Time
70 phút.

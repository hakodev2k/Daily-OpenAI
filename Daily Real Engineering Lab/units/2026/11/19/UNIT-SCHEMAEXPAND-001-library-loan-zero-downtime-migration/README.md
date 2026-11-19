# UNIT-SCHEMAEXPAND-001 — Thay đổi schema không gián đoạn hệ thống mượn sách
## Mục tiêu
Điều tra sự cố triển khai schema trong hệ thống đang có traffic, lập kế hoạch migration và chứng minh khả năng rollback bằng bằng chứng.
## Bối cảnh thực tế
Dịch vụ quản lý thư viện có API .NET 8 và SQL Server. Release mới yêu cầu thêm mã định danh người mượn chuẩn hóa, trong khi phiên bản cũ và mới cùng hoạt động 30 phút. Một lần thử triển khai ở staging khiến p95 tăng và một số requests timeout. Không được dừng dịch vụ.
## Bạn cần làm gì
Đọc [tình huống và số liệu](evidence/incident.md); lập phương án migration trong [workspace](workspace/my-investigation.md), thực hiện tabletop replay theo [kịch bản](docs/replay.md), ghi các phép đo và quyết định go/no-go. Chỉ xem [gợi ý](hints/hint-01.md) khi bế tắc.
## Yêu cầu môi trường
Không cần Azure, SQL Server hay tài khoản trả phí. Markdown editor và PowerShell 7 (tùy chọn) đủ để hoàn thành tabletop. Các con số trong evidence là dữ liệu mô phỏng, không phải benchmark đã chạy.
## Chạy nhanh
Mở evidence/incident.md và docs/replay.md, điền workspace/my-investigation.md. Dùng bảng trạng thái ở replay để kiểm tra từng bước với hai phiên bản ứng dụng.
## Cách reproduce vấn đề
Tại trạng thái S0 trong replay, áp dụng migration M0 và đối chiếu timeline trong evidence. Phân loại các thao tác bị chặn, request lỗi và cửa sổ rollback.
## Những gì cần quan sát
Phân biệt thời gian chờ lock, thời gian chạy DDL, throughput và tính tương thích của cả hai phiên bản. Xác định trạng thái dữ liệu có thể phục hồi khi triển khai dở.
## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.
## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)
## Reference Solution
**Spoiler:** [Đối chiếu sau khi tự làm](solution/README.md).
## Expected Results
Trước: M0 không đáp ứng SLO và rollback không an toàn. Sau: phương án đề xuất duy trì tương thích đọc/ghi và có gate định lượng ở mỗi bước.
## Estimated Time
65 phút (D4 / L5).

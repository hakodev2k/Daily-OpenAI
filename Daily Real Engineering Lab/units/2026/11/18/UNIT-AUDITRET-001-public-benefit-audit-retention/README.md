# UNIT-AUDITRET-001 — Hồ sơ trợ cấp: chọn chiến lược lưu trữ audit 7 năm

## Mục tiêu
Đưa ra Architecture Decision Record (ADR) có thể bảo vệ bằng số liệu, phân biệt dữ liệu nghiệp vụ hiện hành và chứng cứ audit.

## Bối cảnh thực tế
Hệ thống xét duyệt trợ cấp công có 1,8 triệu hồ sơ/năm. Truy vấn hồ sơ đang chậm dần; kiểm toán yêu cầu truy xuất lịch sử thay đổi trong 7 năm. Một nhóm đề nghị tăng tài nguyên database, nhóm khác muốn chuyển toàn bộ audit sang object storage.

## Bạn cần làm gì
Đọc [bối cảnh và ràng buộc](docs/scenario.md), đối chiếu [bằng chứng](evidence/incident.md), viết ADR vào [workspace](workspace/my-decision.md). Đưa ra ít nhất hai phương án, mô hình dung lượng, security, RPO/RTO, kiểm chứng và rollback. Sau đó xử lý thay đổi yêu cầu ở cuối scenario.

## Yêu cầu môi trường
Chỉ cần trình soạn thảo Markdown, máy tính và PowerShell tùy chọn. Không cần cloud, database hay dịch vụ trả phí.

## Chạy nhanh
Từ thư mục unit: mở `docs/scenario.md`, `evidence/incident.md`, điền `workspace/my-decision.md`. Kiểm tra bằng checklist ở `docs/acceptance.md`.

## Cách reproduce vấn đề
Đây là Design Decision Lab, không có lỗi executable để reproduce. Tái dựng quyết định bằng dữ liệu trong evidence; tính dung lượng 7 năm và thử đánh giá phương án khi thay đổi requirement.

## Những gì cần quan sát
Các phương án có thể đáp ứng latency hiện tại nhưng khác nhau về truy xuất audit, vòng đời dữ liệu, phục hồi và chi phí vận hành. Phải ghi rõ dữ kiện còn thiếu thay vì tự suy đoán.

## Quy tắc làm lab
1. Đọc scenario và evidence trước.
2. Ghi hypotheses và assumptions.
3. Đề xuất, định lượng và phản biện phương án.
4. Đối chiếu acceptance.
5. Chỉ sau đó xem reference solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Một phương án có thể bảo vệ](solution/README.md). Chỉ xem sau khi tự viết ADR.

## Expected Results
Một ADR có lựa chọn, bảng trade-off, tính toán có đơn vị, threat model, migration, validation, rollback và câu trả lời cho requirement change.

## Estimated Time
65 phút.
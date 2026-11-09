# Self-verification — chỉ mở sau khi tự viết ADR

Chấm mỗi tiêu chí 0 (thiếu), 1 (nêu nhưng không kiểm chứng được), 2 (có bằng chứng, owner và cách kiểm chứng).

1. **Consumer coverage:** xác định owner của K05, xử lý K04 dormant, không coi traffic 0 là đã migrate.
2. **Contract correctness:** v1/v2 field mapping, identity uniqueness, receipt reconciliation và replay semantics.
3. **Cutover safety:** K03 offline đến T+34 và K04 dormant đến T+50 vẫn được xử lý theo chính sách đã chọn.
4. **Operational gates:** dashboard version/client/status, lỗi, mismatch receipts, unknown consumers, canary và deadline.
5. **Rollback:** khả năng bật lại route và dữ liệu tương thích; rollback drill trước khi sunset.
6. **Trade-offs:** A/B/C có cost, độ phức tạp, rủi ro và lý do chọn.
7. **Requirement change:** cập nhật quyết định khi K03 không nâng cấp trước T+60.
8. **Governance:** owner, escalation, lịch thông báo và phê duyệt từ Product/Compliance.

Tổng 16 điểm; 14+ là mạnh nếu không có vi phạm bắt buộc. **Hard fail** nếu giải pháp làm mất 900 giao dịch K03, làm mất K04 hoặc tạo duplicate receipt không có kiểm soát. Bằng chứng phải mô tả cơ chế test được, không chỉ lời hứa.

Không cần một kiến trúc cụ thể: phương án khác vẫn đạt nếu vượt qua mọi hard gate.

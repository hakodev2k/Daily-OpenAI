# Senior challenge — mixed-version migration

40% replica vẫn phát numeric milliseconds nhưng khai báo seconds; 60% đã sửa. Cùng metric name đang được tổng hợp chung, không thể dừng API.

Viết ADR một trang: lựa chọn versioned metric hoặc dual-publish, canary, alert cutover, xử lý lịch sử, rollback, tiêu chí kết thúc migration. So sánh ít nhất hai phương án và nêu rủi ro cardinality. Acceptance: không trộn số khác unit trong cùng query, không mất alert thật và có audit trail cho false historical breaches.

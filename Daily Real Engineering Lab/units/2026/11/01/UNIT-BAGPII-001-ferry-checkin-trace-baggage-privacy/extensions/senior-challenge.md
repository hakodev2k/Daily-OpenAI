# Senior challenge — thay đổi yêu cầu

Một nhà cung cấp tracing mới muốn nhận thêm `booking_reference` để hỗ trợ support; đội bảo mật yêu cầu giới hạn truy cập theo vai trò và xóa telemetry sau 30 ngày. Thiết kế policy cho classification, baggage allowlist, trace correlation, retention, vendor contracts, incident containment và kiểm thử E2E. Viết ADR ngắn so sánh: opaque ID + secure lookup, salted pseudonymization, vendor-side redaction. Chỉ ra residual risks và rollback plan.

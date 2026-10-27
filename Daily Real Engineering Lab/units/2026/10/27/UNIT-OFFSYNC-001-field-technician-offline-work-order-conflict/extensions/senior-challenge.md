# Senior challenge — productionize sync contract

1. Thiết kế bảng `WorkOrderFields` với `(TenantId, WorkOrderId, FieldName, FieldRevision)` và bảng `AppliedChanges` với unique key theo tenant + change ID. Đề xuất transaction / optimistic CAS đảm bảo hai request cùng BaseFieldRevision không cùng thành công.
2. Xử lý crash ngay sau khi commit nhưng trước khi trả HTTP response: retry phải trả lại outcome ổn định, không double-apply.
3. Bổ sung state-machine validation cho Status; field-level revision là điều kiện cần nhưng không thay thế business invariant.
4. Phân biệt `409 Conflict` (base stale hoặc ID reuse) và successful idempotent replay. Đề xuất response chứa authoritative field revision, current value và conflict metadata nhưng không rò dữ liệu tenant.
5. Thảo luận tombstone khi field bị xóa, thời gian lưu idempotency key và migration khi thêm field mới. Nêu giới hạn của CRDT/LWW so với human resolution.

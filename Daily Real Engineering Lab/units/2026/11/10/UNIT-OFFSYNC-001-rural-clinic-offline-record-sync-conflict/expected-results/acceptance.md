# Acceptance checklist

- Reproduce chính sách cũ: cuối cùng allergy=none, medication=amoxicillin, note=requires-follow-up khi mỗi full-document PUT ghi đè document và B có clock lớn hơn A.
- Chỉ ra nguy cơ clinical data loss và không dùng client timestamp làm authority.
- Với chính sách mới: A1 từ revision 7 thành revision 8; B1/B2 từ revision 7 không được âm thầm overwrite; retry A1 trả lại kết quả trước đó.
- Nếu merge field độc lập, phải chứng minh field mask + baseRevision; allergy/medication phải qua review.
- 409 có serverRevision, serverSnapshot (redacted theo quyền), submittedChange, conflictFields và correlationId.
- Có audit immutable theo operationId; không ghi thông tin bệnh nhân vào log/trace.
- Kiểm thử ít nhất: concurrent edits, duplicate retry, clock skew, out-of-order delivery, reconnect burst, auth scope, rollback.
- SLO/metrics: conflict rate, dedup hit, stale base rate, review queue age, sync latency p95; có cảnh báo khi mất audit.
- Đưa ra ít nhất hai phương án và trade-off, cùng kế hoạch rollout theo canary.

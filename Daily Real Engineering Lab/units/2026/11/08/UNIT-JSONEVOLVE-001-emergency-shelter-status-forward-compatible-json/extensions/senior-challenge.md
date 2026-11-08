# Senior challenge

Thiết kế phiên bản ingest hỗ trợ partial success trên batch 10.000 records, bảo đảm idempotency theo `(providerId, eventId)`, có dead-letter cho payload malformed, giới hạn metric cardinality và có policy khi tỷ lệ Unknown vượt 5%. Viết ADR ngắn so sánh reject-all và per-record quarantine.

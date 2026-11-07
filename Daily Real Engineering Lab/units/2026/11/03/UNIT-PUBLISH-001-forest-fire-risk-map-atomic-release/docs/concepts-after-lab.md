# Concepts after lab

- Multi-object writes không có atomicity tự nhiên trong object store.
- Immutable versioned objects + atomic manifest/pointer switch tạo publish boundary rõ ràng.
- Reader phải pin một release cho cả request; không đọc lại pointer cho từng tile.
- Manifest nên chứa checksum, expected object list, schema version và validation status.
- Concurrent publisher cần compare-and-swap/ETag hoặc lease có fencing.
- CDN cache key nên chứa release; pointer cache có TTL thấp và được kiểm soát.
- Rollback là đổi pointer sang manifest đã xác thực, không overwrite tile hiện tại.

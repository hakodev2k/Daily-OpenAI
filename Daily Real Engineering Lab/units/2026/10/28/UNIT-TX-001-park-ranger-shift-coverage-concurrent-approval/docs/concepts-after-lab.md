# Concepts after lab

**Write skew** có thể xảy ra khi hai transaction quan sát cùng một snapshot hợp lệ và thay đổi hai row khác nhau, khiến một invariant toàn cục bị vi phạm sau khi cả hai commit. Cơ chế cụ thể phụ thuộc DBMS, isolation level, constraint và phạm vi đọc/ghi.

**Atomic per-row update ≠ atomic business decision.** Quy tắc toàn ca phải được kiểm tra và thực thi tại cùng ranh giới authoritative write.

**Serialization point** là thời điểm các quyết định cạnh tranh được sắp thứ tự logic. Với nhiều replica, lock nội bộ của một process không tạo serialization point chung.

**Optimistic concurrency** chỉ có tác dụng khi hai quyết định cạnh tranh trên cùng một version hoặc cùng một predicate được DB bảo vệ. Version riêng cho từng nhân viên không đủ bảo vệ minimum coverage.

**Verification pyramid:** fixture deterministic → integration test với DB thật và hai connections → concurrency stress test → production observability. Fixture không thay thế integration test.

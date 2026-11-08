# Verification checklist

- [ ] Reproduce chính xác trạng thái E6 theo rule cũ.
- [ ] Nêu invariant và ba phương án có trade-offs.
- [ ] Định nghĩa commandId, expectedRevision, serverRevision, 409.
- [ ] Giải thích T1–T8, kể cả duplicate và clock skew.
- [ ] Rollout, rollback, privacy và metrics có thể đo.
- [ ] Không dùng client timestamp làm thứ tự authoritative.

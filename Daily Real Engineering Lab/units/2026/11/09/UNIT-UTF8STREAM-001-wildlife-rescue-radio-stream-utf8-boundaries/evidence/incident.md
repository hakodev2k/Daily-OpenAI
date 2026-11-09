# Incident evidence (synthetic)
- Gateway packet capture: payload bytes identical across attempts; checksum validated at ingress.
- Receiver logs: some location names contain U+FFFD replacement character.
- Reproduction matrix: one large read passes; split read sometimes corrupts names; ASCII-only tests pass.
- Hypotheses to evaluate: corrupted source bytes; storage encoding mismatch; transport chunk boundaries; JSON escaping.
- Không có dấu hiệu mất byte hoặc đảo thứ tự byte. Chưa kết luận root cause từ log ứng dụng đơn lẻ.


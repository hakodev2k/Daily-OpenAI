# Before — Chưa có quyết định kỹ thuật được kiểm chứng

- Một lần thay mapping trực tiếp trên search path từng đi kèm tăng p95 và lỗi 5xx.
- Chưa có bằng chứng snapshot + thay đổi trong thời gian backfill được hòa giải đúng.
- Chưa có cutover gate, canary hay rollback drill.
- `run.ps1` cho thấy baseline có thể catch up; campaign-surge và replay-degraded không thể tự catch up với throughput đã nêu.
- Không kết luận một công nghệ là đúng chỉ vì phổ biến; hãy so sánh rủi ro theo constraint.

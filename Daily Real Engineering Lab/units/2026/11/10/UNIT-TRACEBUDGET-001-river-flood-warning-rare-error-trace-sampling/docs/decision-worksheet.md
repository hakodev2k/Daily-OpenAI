# Decision worksheet

| Candidate | Rule | Cần tính |
|---|---|---|
| A | Head sample đồng nhất 5% | error traces kỳ vọng, total traces |
| B | Head sample đồng nhất 8% | error traces kỳ vọng, total traces |
| C | Giữ 100% error traces sau outcome + lấy ngẫu nhiên 5% success traces | total traces, điều kiện khả thi |
| D | Giữ 100% error traces + lấy ngẫu nhiên 8% success traces | total traces, ngân sách |

Lưu ý: 8% của toàn bộ 1.000.000 là 80.000; 8% successes cộng với 100% errors là một phép tính khác.

## ADR cần nộp
- Context: triệu chứng, ảnh hưởng P0, dữ liệu còn thiếu.
- Decision: chọn candidate hoặc hybrid; lý do định lượng.
- Alternatives: vì sao không chọn từng phương án còn lại.
- Operational guardrails: queue pressure, collector health, propagation, PII redaction, latency, sampling config rollout.
- Verification: 4 phép tính và 5 invariants; metrics đo trước/sau.
- Rollback: ngưỡng kích hoạt và cách quay về baseline không ảnh hưởng delivery.

## Invariants
1. Không vượt quota retained traces trong cửa sổ mô phỏng.
2. Metrics đếm request/error độc lập với trace sampling.
3. Trace giữ phải đủ parent-child spans; thiếu span được đo riêng.
4. Telemetry không chứa dữ liệu cá nhân trong attributes hoặc payload.
5. Telemetry pipeline không làm request xử lý cảnh báo bị lỗi.

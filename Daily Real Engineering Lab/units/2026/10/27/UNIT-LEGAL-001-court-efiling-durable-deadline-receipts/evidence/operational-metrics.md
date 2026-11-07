# Evidence bundle — production snapshot (synthetic)

Dữ liệu tổng hợp cho tabletop; **không phải** telemetry từ hệ thống thật.

| Signal | Baseline | Incident window |
|---|---:|---:|
| Submission API 2xx | 99.8% | 99.6% |
| Receipt lookup missing after 2xx | 0 | 3 |
| Duplicate receipt IDs for same filing intent | 0 | 2 |
| Authoritative store commit confirmation p95 | 180 ms | 940 ms |
| Replica acknowledgement delay p99 | 120 ms | > 5 s / unknown |
| Scanner queue age p99 | 4 min | 45 min |
| CPU utilization | 45% | 52% |
| Gateway retries | 0.2% | 3.7% |

## Log extracts

```text
16:59:58.520 gateway request=req-A status=200 receipt=R-204
16:59:58.690 failover site=primary reason=power-loss
17:00:00.050 store request=req-B commit=observed receipt=R-883
16:59:56.500 api request=req-C2 receipt=R-110 previous-intent=party-42:filing-109
16:59:57.600 gateway request=req-E status=200 durable-ack=unknown
```

## Các giả thuyết cạnh tranh

- Có thể gateway tạo thêm request ngoài ý muốn.
- Có thể scan chậm khiến lookup trả rỗng.
- Có thể logic xác định deadline dựa trên một mốc thời gian không nhất quán.
- Có thể đường đi phản hồi và đường đi xác nhận lưu trữ không có cùng một hợp đồng.

**Bài tập:** nêu evidence cần thu thêm để loại trừ từng giả thuyết. Không kết luận chỉ từ tỷ lệ HTTP 2xx.

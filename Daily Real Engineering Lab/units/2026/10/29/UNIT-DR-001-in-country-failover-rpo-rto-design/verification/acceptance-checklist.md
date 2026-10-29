# Self-verification rubric

| Gate | PASS khi |
|---|---|
| V1 | Đánh giá P1–P6 riêng biệt, không bù điểm cho policy violation |
| V2 | Đủ A–E, đủ ba drill C |
| V3 | Phân biệt observed và guaranteed RPO/RTO, không dùng p95 làm upper bound |
| V4 | Tính số giây C3 vượt RPO |
| V5 | Có durable commit position, replica position, fencing epoch |
| V6 | Có chính sách khi lag vượt safety margin và nêu write-availability trade-off |
| V7 | RTO tính đến safe writes resumed |
| V8 | Có peak-load test và all-in cost |
| V9 | So sánh ít nhất ba phương án và đưa ra quyết định có điều kiện |
| V10 | Có failback, repeatable drill, owner, data-integrity checks |

Thiếu V1, V3, V5 hoặc V6 → NO-GO cho production sign-off. Đây là đánh giá tư duy kiến trúc, không phải chứng nhận hạ tầng thật.

# Incident timeline

- 10:00:00 UTC: hộp BOX-42 bắt đầu bàn giao; process monotonic counter = 0ms.
- Trong 3 giây tiếp theo: đồng hồ host được hiệu chỉnh lùi 5 giây.
- 09:59:58 UTC: bàn giao kết thúc; monotonic counter = 3000ms.
- Dashboard ghi elapsed = -2000ms, nhưng hệ thống theo dõi vật lý ghi 3000ms.

Nhiễu điều tra: không có retry, request timeout hay mất kết nối. Không suy luận nguyên nhân chỉ từ dashboard; đối chiếu hai nguồn đo.

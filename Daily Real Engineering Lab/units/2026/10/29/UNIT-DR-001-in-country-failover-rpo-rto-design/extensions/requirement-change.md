# Senior challenge — đổi yêu cầu

Product giảm RPO từ 300 giây xuống **60 giây**, giữ P1/P3/P4/P5/P6 và giới hạn write-unavailable do lag dưới 2 phút/tháng.

1. Tái đánh giá A–E, không tự giả định lag giảm.
2. Xác định yêu cầu có khả năng xung đột và bằng chứng thiếu.
3. Đưa ít nhất hai hướng: cải thiện replication trong X, thay đổi write contract hoặc thương lượng lại SLO.
4. Đề xuất failure injection: X2 chậm 90 giây, X1 mất đột ngột, old primary quay lại.
5. Viết ADR revision có owner phê duyệt từng trade-off.

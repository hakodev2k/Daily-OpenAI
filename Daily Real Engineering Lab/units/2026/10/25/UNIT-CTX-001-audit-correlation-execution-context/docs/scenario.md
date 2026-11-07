# Bối cảnh và investigation

Fraud Risk nhận hai request đồng thời, gắn correlation ID từ ingress và ghi audit receipt sau khi công việc được chuyển cho worker nội bộ. Event ID và category luôn đúng, nhưng hệ thống tìm kiếm audit theo correlation ID trả về rỗng.

**Dữ kiện:** cùng một request, một phép tính async kiểm soát vẫn giữ correlation ID. Không có HTTP request thật, không có database và không có sampling trong simulator; đây là mô hình tối giản của một boundary thường gặp trong backend.

Thu thập: (1) output của `--inspect`; (2) output `--reproduce`; (3) kết quả `--verify`; (4) giải thích tại sao việc tăng số worker, thêm delay hay thay đổi thứ tự request không đủ để bảo đảm correctness.

Hypothesis gợi ý để tự kiểm tra: race giữa request A/B; mất dữ liệu ở audit queue; sự khác biệt về phạm vi ngữ cảnh của hai đường async. Chỉ chọn hypothesis sau khi có evidence.

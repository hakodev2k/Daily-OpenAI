# Hợp đồng đo lường và chính sách hiện tại

SLI: tỷ lệ request hợp lệ trả về kết quả thành công cho endpoint `GET /catalog/search` và `POST /holds`; HTTP 5xx, timeout từ gateway và 429 do chính dịch vụ áp đặt được tính là lỗi. Loại health probes, traffic synthetic và lỗi 4xx do client nhập sai. Đây là giả định của bài lab; khi triển khai thật cần kiểm chứng với product owner.

SLO rolling 30 ngày: **99.9%** request tốt. Error budget = 0.1% tổng request hợp lệ. Burn rate = (errors / requests) / 0.001. Ví dụ burn=10 nghĩa là tiêu thụ budget nhanh gấp 10 lần mức cho phép trung bình.

Policy cũ: **page on-call nếu bất kỳ cửa sổ 5 phút có error ratio > 2%**, không kiểm tra số request, không so sánh với cửa sổ dài, không có ticket tier.

CSV là các cửa sổ **độc lập dùng để so sánh phương án**, không cộng các dòng 5m và 1h như các khoảng rời nhau. Cột incident mô tả nhóm sự cố; cùng incident có các cửa sổ 5m, 1h, 6h. `impact` là đánh giá vận hành dựa trên hỗ trợ khách hàng, không phải ground truth để hard-code alert.

Ràng buộc: page không quá 2 lần mỗi tuần trong tuần ổn định; phát hiện sự cố ảnh hưởng rộng trong khoảng 10 phút; không bỏ qua sự cố kéo dài. Không có latency histogram trong bộ dữ liệu: phải nêu giới hạn coverage.

## Việc cần giao nộp
- Bảng tính error ratio, burn rate và kết quả policy cũ cho mọi dòng.
- Policy mới gồm biểu thức ngưỡng, cửa sổ, minimum traffic, routing và anti-flap.
- Đánh giá theo incident, bao gồm false positives và detection latency.
- Runbook và rollout: shadow evaluation 7 ngày, owner, rollback và dashboard.

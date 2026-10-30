# Senior challenge — requirement change
Sau 2 tháng, số sensor tăng gấp 10, có 20.000 depot và 4 region. Budget 5.000 series không đổi; product yêu cầu alert theo depot cho **top 200 depot quan trọng**, còn lại theo region.

Hãy viết ADR tối đa một trang:
- định nghĩa bounded labels/metric families và cách chọn top 200 ổn định;
- tính upper bound series theo outcome và vùng;
- đảm bảo không tạo churn lớn khi top-200 thay đổi;
- đề xuất alert quality, sampling, logs/traces, quyền truy cập và canary rollback;
- giải thích tại sao phương án giữ tất cả depot không còn đáp ứng budget.
Không có một đáp án kiến trúc duy nhất; mọi phương án phải lượng hóa giới hạn và chứng minh operational requirements.

# Requirement change — sau khi chốt ADR

45 phút sau khi phương án được review, Product Owner thêm ràng buộc:

1. Sáu cơ sở có thể mất WAN trong **4 giờ** nhưng vẫn cần ghi local audit và cho phép một số thao tác nghiệp vụ ngoại tuyến.
2. Sau khi kết nối lại, event arrival order khác event occurrence order.
3. Ban tuân thủ yêu cầu xác định rõ thời gian nào **chưa có bằng chứng độc lập**; không được hiển thị màu xanh giả.
4. Dữ liệu legacy 18 tháng phải import, nhưng không có checkpoint độc lập trước đây.

Hãy cập nhật ADR: ordering scope, offline local trust anchor, conflict/duplicate reconciliation, proof-gap reporting, recovery SLA, legacy provenance và quy tắc chặn/mở release. Chỉ xem [reference](../solution/README.md) sau khi tự đề xuất.

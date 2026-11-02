# Investigation guide

1. Tổng hợp CSV; kiểm tra mỗi vùng 80 và toàn pilot 240.
2. Tính tỷ lệ failed trên targeted và trên terminal; nêu mẫu số phù hợp với failure gate.
3. Vẽ sequence cho c-014 và n-044. Xác định event identity, attempt identity và điều kiện ghi đè state.
4. Mô tả race khi scheduler cấp lệnh cho hai device cùng site.
5. Tính tổng MiB/GiB cho full/delta; đánh giá 48 giờ khi offline 30%.
6. Liệt kê điều không thể bảo đảm: rollback ngay lập tức trên offline/single-bank, 100% completion khi mất kết nối, biết trạng thái chỉ từ command dispatch.
7. Viết ADR có giả định, observability, blast radius, failure injection và khi nào phải đổi quyết định.

Không coi dashboard xanh là bằng chứng duy nhất; chỉ rõ nguồn sự thật và độ trễ dữ liệu.

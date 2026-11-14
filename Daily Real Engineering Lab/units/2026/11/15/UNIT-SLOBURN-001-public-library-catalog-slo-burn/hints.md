# Progressive hints

<details><summary>Hint 1</summary>

Đừng chỉ xem phần trăm: 1/20 và 300/5000 mang ý nghĩa thống kê và vận hành rất khác nhau. So sánh số request và độ dài cửa sổ.

</details>

<details><summary>Hint 2</summary>

Tính burn rate bằng tỷ lệ lỗi chia 0.001. Xem đồng thời một cửa sổ ngắn và dài để tránh page vì spike ngắn không bền vững.

</details>

<details><summary>Hint 3</summary>

Thử page khi burn >= 14.4 ở cả 5m và 1h, hoặc >= 6 ở cả 30m và 6h. Bộ dữ liệu thiếu 30m nên phải nêu rằng không thể verify đầy đủ nhánh thứ hai. Ticket cho mức suy giảm kéo dài; tách low traffic sang absolute-error/heartbeat và review thủ công.

</details>

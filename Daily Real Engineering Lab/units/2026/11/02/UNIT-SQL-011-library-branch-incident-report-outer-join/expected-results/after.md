# After — learner-edited starter

- Báo cáo ban đầu gồm Central = 2, West = 0, North = 0, East = 0; thứ tự theo ID.
- Sau khi thêm South với sự cố chỉ ở trạng thái `Closed`, South vẫn có mặt với giá trị 0.
- Sau khi thêm một sự cố `Open` + `Critical` tại West, West = 1 và các chi nhánh khác giữ nguyên.
- `./verify.ps1` trả exit code 0 chỉ khi cả ba trạng thái đều đạt yêu cầu.
- Các bộ lọc trong truy vấn không làm mất chi nhánh không có sự cố phù hợp.

# Before — starter

- Bảng `Branches` có bốn chi nhánh, bảng `Incidents` có sáu bản ghi.
- Có đúng hai sự cố `Open` + `Critical` trong database.
- `./run.ps1` chỉ in `Central | 2` (một hàng), thiếu West, North, East.
- `./reproduce.ps1` trả exit code 0 khi xác nhận được chính xác triệu chứng đó.
- Không dùng thời gian chạy làm tiêu chí; sự khác biệt là số hàng và ý nghĩa của số 0.

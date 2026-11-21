# Evidence snapshot (mô phỏng)

- 09:00 UTC: chuyển một nhóm worker sang region B, image và raw payload hash giữ nguyên.
- 09:04 UTC: tỷ lệ `REJECT` tăng; một số thiết bị vẫn hoạt động bình thường.
- 09:06 UTC: kiểm tra mạng không thấy retry hay mất frame.
- Sample: `S-41|12.75`, `S-42|-3.50`.
- Giả thuyết cạnh tranh: schema drift, lỗi network, khác biệt cấu hình tiến trình, phép tính hậu xử lý.
- Yêu cầu: kiểm chứng giả thuyết bằng replay cùng payload trước khi sửa.

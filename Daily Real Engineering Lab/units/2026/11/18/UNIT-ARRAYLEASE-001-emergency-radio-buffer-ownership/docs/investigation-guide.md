# Điều tra có bằng chứng
1. Chạy reproduce 2 lần, lưu cặp accepted/transmitted.
2. Đặt breakpoint tại Enqueue, Return và Drain. Ghi lại object identity.
3. Đề xuất hai hypothesis: encoding và buffer lifetime. Dùng evidence để loại từng giả thuyết.
4. Nêu invariant: mỗi job phải phát đúng bytes tại thời điểm tiếp nhận.
5. Sửa starter; verify cả hai message; giải thích vì sao một message duy nhất không bắt được regression.

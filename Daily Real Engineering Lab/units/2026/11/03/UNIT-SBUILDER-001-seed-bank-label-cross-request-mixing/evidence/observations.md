# Dữ liệu quan sát cần thu thập

Timeline do harness điều khiển, không sử dụng `Task.Delay`:

| Mốc | Hành động | Câu hỏi cần kiểm tra |
|---|---|---|
| T1 | Bắt đầu tạo nhãn A, chờ phí A | A đã ghi phần nào? |
| T2 | Bắt đầu tạo nhãn B, chờ phí B | B có ảnh hưởng đến A không? |
| T3 | Gateway trả phí B; nhãn B hoàn thành | B có đúng hợp đồng không? |
| T4 | Gateway trả phí A; nhãn A hoàn thành | A có chứa dữ liệu lô khác không? |

Hai giá trị gateway cố định: LOT-A = 17.00, LOT-B = 42.00. Dùng log từ `./run.ps1` để phân biệt lỗi nguồn dữ liệu với lỗi tạo kết quả. Đừng đọc reference solution trước khi ghi hypothesis.

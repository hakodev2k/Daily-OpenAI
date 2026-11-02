# Bối cảnh và contract nghiệp vụ

Một dashboard của phòng kiểm định nước thải đọc bảng `Inspections` để hiển thị hai nhóm `Open` và `Closed`. Người trực ca cần nhìn thấy tất cả mẫu được seed trong lần chạy thử; kết quả không được thiếu, trùng hoặc phân loại sai.

## Dữ liệu kiểm thử

| TicketId | Status |
|---|---|
| WW-101 | Open |
| WW-102 | Closed |
| WW-103 | Open |

## Ràng buộc

- Chạy local bằng SQLite file tạm, không cần cloud.
- Được phép tổng hợp hai nhánh đồng thời hoặc tuần tự, nhưng phải giải thích trade-off.
- Không thay đổi bảng, dữ liệu, bộ kiểm thử hoặc contract trả về.
- Khi có lỗi, không được phát hành báo cáo thiếu một phần như thể thành công.
- Kết quả phải deterministic dù máy chạy nhanh hay chậm.

## Bằng chứng cần ghi

Exception type/message; thứ tự khởi chạy; ranh giới I/O đang chờ; số lượng và mã ticket trước/sau. Ghi ít nhất ba giả thuyết trước khi mở hint.

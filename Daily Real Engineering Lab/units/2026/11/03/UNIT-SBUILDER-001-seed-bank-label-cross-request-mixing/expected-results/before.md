# Before — starter nguyên bản

- `./reproduce.ps1` exit 0: **đã chứng minh symptom**.
- `./verify.ps1` exit 1: hợp đồng regression chưa đạt.
- `Sequential contract: PASS`.
- `Overlapping contract: FAIL`.
- Nhãn A chứa dữ liệu của LOT-B; B hoàn thành trước A theo thứ tự điều khiển.
- Không phụ thuộc vào thời gian ngủ hay mạng; chạy lại cho cùng loại sai lệch.

Nếu không tái hiện, xác nhận bạn chưa sửa `starter/Program.cs`, SDK build thành công và dùng đúng script trong thư mục unit.

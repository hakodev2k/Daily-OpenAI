# After — policy đã được sửa
- `./verify.ps1` exit code 0, mọi test `PASS`.
- Message ASCII và Unicode có wire payload đúng 16 byte vẫn được chấp nhận.
- Message vượt giới hạn bị chặn tại local policy, không phát request dẫn đến 413.
- Empty message và argument guard vẫn giữ nguyên hành vi.
- `./reproduce.ps1` sau fix sẽ trả non-zero: triệu chứng ban đầu không còn xuất hiện. Đây là **đúng contract**, không phải lỗi verify.

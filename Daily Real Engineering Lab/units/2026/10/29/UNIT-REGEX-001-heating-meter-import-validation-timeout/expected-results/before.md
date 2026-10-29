# Before — starter chưa sửa
- `ordinary-valid`, `long-valid` được chấp nhận.
- `hyphen-invalid`, `empty-invalid` bị từ chối.
- `long-invalid` làm validation ném `RegexMatchTimeoutException` khi chạy với timeout của starter.
- `./reproduce.ps1` ghi `EXPECTED_FAILURE_REPRODUCED` và trả exit code 0 khi triệu chứng đúng; `./verify.ps1` trả lỗi.
- Nếu môi trường có hành vi khác, kiểm tra SDK .NET 8, chuỗi test, timeout và build Release; không xem một lần chạy nhanh là bằng chứng fix.

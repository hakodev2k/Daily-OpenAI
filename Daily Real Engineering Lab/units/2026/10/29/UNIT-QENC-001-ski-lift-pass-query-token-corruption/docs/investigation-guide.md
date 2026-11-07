# Investigation Guide

Thu thập bằng chứng từ `./run.ps1`, `./reproduce.ps1` và các case trong `starter/Program.cs`.

1. Phân loại lỗi: xác thực vé, transport, hay serialization/contract?
2. So sánh chuỗi issuedPass, request URI và chuỗi partner tiếp nhận, theo thứ tự byte/ký tự.
3. Đưa ra hai hypothesis có thể bác bỏ (ví dụ hết hạn hoặc dữ liệu thay đổi khi vận chuyển).
4. Thiết kế thử nghiệm tối thiểu thay đổi một thuộc tính của pass mỗi lần.
5. Sửa duy nhất ranh giới tạo URI, giữ partner simulator và issuedPass nguyên trạng.
6. Verify cả positive case lẫn các ký tự đặc biệt; ghi lại regression evidence.

Không cần gọi partner thật, gửi token thật hoặc tạo credential.

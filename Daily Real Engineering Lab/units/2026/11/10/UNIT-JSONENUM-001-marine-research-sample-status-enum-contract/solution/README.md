# Reference Solution

## Root cause
`JsonStringEnumConverter` mặc định nhận integer JSON. Giá trị 99 deserialize thành enum có underlying value 99 dù không có member tương ứng. Parser thành công không đồng nghĩa hợp đồng business hợp lệ.

## Fix
Xem [Program.cs](Program.cs): tắt `allowIntegerValues` và kiểm tra `Enum.IsDefined`. Không thay đổi hai trạng thái string hợp lệ.

## Verification
Áp dụng thay đổi vào `starter/Program.cs`, chạy `./verify.ps1`; 4 PASS và exit 0. Script verify không chạy solution thay cho starter.

## Wrong fixes
- Thêm default trong switch: payload lạ vẫn vào hệ thống.
- Kiểm tra `Status != 0`: 99 vẫn lọt.
- Catch rồi mặc định `Ready`: có thể đưa mẫu chưa duyệt vào phân tích.

## Trade-offs
Reject giá trị lạ bảo vệ integrity nhưng partner có thể mở rộng enum. Cần versioning, quarantine và log metadata an toàn thay vì tự động đoán trạng thái.

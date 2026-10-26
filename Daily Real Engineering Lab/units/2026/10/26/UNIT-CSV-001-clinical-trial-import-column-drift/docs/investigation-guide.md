# Investigation guide (không tiết lộ solution)

1. Dùng `./run.ps1` lấy mẫu output, ghi số record và số cột.
2. Mở `starter/TrialImport/Program.cs` để biết hợp đồng đầu vào/đầu ra và fixture.
3. Đối chiếu các boundary trong `CsvParser.ParseRecords` với dữ liệu gốc.
4. Đề xuất tối thiểu hai giả thuyết: do encoding/line ending, hay do cách nhận diện field/record?
5. Sửa `starter/TrialImport/CsvParser.cs` mà không đổi expected fixture/harness.
6. Chạy `./verify.ps1`, kiểm tra lỗi input không hợp lệ và hành vi khi file kết thúc không có newline.

Không sửa `Program.cs` hoặc scripts để làm test pass; test là hợp đồng hành vi.

# UNIT-HMACBYTES-001 — Webhook chữ ký hợp lệ nhưng bị từ chối theo định dạng JSON

## Mục tiêu
Điều tra sự cố xác thực webhook bằng evidence ở mức byte, phân biệt nội dung nghiệp vụ và biểu diễn trên wire; thiết kế regression test chống thay đổi serialization.

## Bối cảnh thực tế
Hợp tác xã thu mua trái cây nhận thông báo chốt phiếu cân từ một đối tác. Sau khi đối tác thay đổi JSON serializer, một phần thông báo trả về 401 dù key không đổi. Đối tác khẳng định chữ ký hợp lệ.

## Bạn cần làm gì
1. Chạy lại hai payload có cùng dữ liệu nghiệp vụ nhưng định dạng khác nhau.
2. Ghi ít nhất ba giả thuyết; thu thập signature, độ dài payload và log quyết định.
3. Sửa `starter/Program.cs` để cả hai payload hợp lệ được chấp nhận nhưng payload bị sửa hoặc chữ ký sai vẫn bị từ chối.
4. Chạy verify và ghi trade-off cho hệ thống thực.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+ trên Windows. Không cần cloud, database hay NuGet ngoài SDK.

## Chạy nhanh
Tại thư mục unit:
```powershell
dotnet --version
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` chạy chương trình với `--mode reproduce` và yêu cầu thấy đúng triệu chứng trước khi sửa. Sau đó sửa `starter/Program.cs`, chạy `./verify.ps1` để kiểm thử chính đường code vừa sửa.

## Những gì cần quan sát
- Cùng một khóa và cùng nội dung nghiệp vụ, hai request có kết quả xác thực khác nhau.
- Signature đầu vào và số byte của request có thể kiểm tra độc lập.
- CPU, clock drift và key rotation là các giả thuyết cần loại trừ bằng bằng chứng.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** Chỉ đọc sau khi tự sửa: [Solution](solution/README.md).

## Expected Results
Trước sửa: reproduction chứng minh một webhook hợp lệ bị 401. Sau sửa: các trường hợp positive và negative trong verify đều PASS.

## Estimated Time
45 phút.

# Investigation guide (không chứa lời giải)
1. Chạy `./reproduce.ps1`, ghi số lượng và EventId.
2. Dùng debugger hoặc tạm thêm logging trong `starter/BadgeImporter.cs` để quan sát độ dài và mã Unicode của từng `PassCode`.
3. So sánh `StringComparer.Ordinal.Equals` với nhận định của người dùng khi nhìn dashboard.
4. Tách hai khái niệm: cùng cách hiển thị và cùng định danh nghiệp vụ.
5. Lập giả thuyết về dữ liệu nhập, quy tắc so sánh, và vị trí đặt chuẩn hóa nếu có.
6. Sau khi sửa, chạy `./verify.ps1`; thử đổi thứ tự nguồn để kiểm tra chính sách first-seen.

Không sửa harness để làm bài kiểm tra xanh. Nếu script không chạy, kiểm tra `dotnet --list-sdks` và cài .NET 8 SDK.

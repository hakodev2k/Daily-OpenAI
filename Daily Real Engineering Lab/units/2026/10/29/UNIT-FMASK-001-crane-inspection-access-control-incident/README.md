# UNIT-FMASK-001 — Cổng kiểm định cần cẩu cho phép xuất biên bản ngoài dự kiến

## Mục tiêu

Điều tra một sai lệch trong kiểm soát quyền truy cập ở dịch vụ .NET, tái hiện bằng kiểm thử độc lập, ghi lại bằng chứng và sửa mà không làm hỏng quyền hợp lệ.

## Bối cảnh thực tế

Một portal kiểm định cần cẩu có các thao tác xem, duyệt và xuất chứng nhận. Sau một đợt triển khai cấu hình action mới, tài khoản chỉ có quyền xem vẫn thực hiện được thao tác xuất. Những action cũ vẫn hoạt động đúng. Hệ thống phải **deny by default** khi action không có chính sách hợp lệ; không có action công khai trong bài này.

## Bạn cần làm gì

1. Tái hiện kết quả trên fixture có cấu hình đầy đủ và fixture thiếu một action.
2. Thu thập decision matrix: action, granted permissions, kết quả thực tế và kỳ vọng.
3. Viết ít nhất hai hypothesis trước khi xem hints.
4. Chỉnh duy nhất mã learner tại starter/PermissionGate.cs.
5. Chạy verification, kiểm tra cả trường hợp thiếu cấu hình, mask rỗng và quyền kết hợp.

## Yêu cầu môi trường

- Windows PowerShell 5.1+ hoặc PowerShell 7+.
- .NET SDK 8 trở lên có thể build target net8.0; không cần database, Docker, NuGet package ngoài SDK hoặc cloud.
- Chạy các lệnh từ thư mục unit.

## Chạy nhanh

~~~powershell
dotnet --version
./run.ps1
./reproduce.ps1
~~~

Nếu PowerShell chặn script theo ExecutionPolicy, có thể chạy trực tiếp:

~~~powershell
dotnet run --project ./checks/Checks.csproj -- --reproduce
~~~

## Cách reproduce vấn đề

Chạy ./reproduce.ps1 trên starter **chưa chỉnh sửa**. Script trả exit code 0 khi đã quan sát được sai lệch dự kiến, không có nghĩa là hệ thống đúng. Sau đó ghi kết quả vào workspace/my-investigation.md. Xem evidence/incident.md để đối chiếu log mô phỏng.

## Những gì cần quan sát

- Action đã cấu hình vẫn phân biệt đúng người được phép và người không được phép.
- Một action mới chưa xuất hiện trong cấu hình lại nhận quyết định ALLOW.
- Quyền được cấp của user không thay đổi giữa hai lần kiểm tra.
- Sau khi sửa, mọi trường hợp trong verification phải PASS và process trả exit code 0.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng ./verify.ps1 (kiểm tra starter/ đã sửa).
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** chỉ xem [solution/README.md](solution/README.md) và [reference code](solution/PermissionGate.cs) sau khi tự sửa.

## Expected Results

[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md)

## Estimated Time

35–45 phút. Maturity L1, difficulty D3; mục tiêu là hiểu contract bảo mật chứ không đoán một dòng code.
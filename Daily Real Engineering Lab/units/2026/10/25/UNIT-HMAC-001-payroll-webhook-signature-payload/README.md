# UNIT-HMAC-001 — Payroll Webhook Signature Mismatch

## Mục tiêu

Điều tra sự khác biệt giữa một callback hợp lệ và một callback bị từ chối; sửa bộ kiểm tra tính toàn vẹn của webhook mà không làm suy yếu khả năng phát hiện dữ liệu bị thay đổi. Bài lab tập trung vào ranh giới dữ liệu của tích hợp HTTP và kiểm thử hồi quy.

## Bối cảnh thực tế

Một hệ thống quản lý bảng lương nhận callback trạng thái từ nhà cung cấp. Cùng một khóa bí mật và cùng một loại sự kiện, một số callback đi qua nhưng các callback khác bị báo signature mismatch. Nhà cung cấp khẳng định tất cả callback đã được ký hợp lệ. Bộ phận vận hành muốn khôi phục độ tin cậy mà vẫn từ chối nội dung bị can thiệp.

Đây là mô phỏng offline: payload và chữ ký đều là dữ liệu giả, không cần kết nối nhà cung cấp thật.

## Bạn cần làm gì

1. Chạy starter và ghi nhận những trường hợp được chấp nhận hoặc bị từ chối.
2. Đưa ra ít nhất hai giả thuyết; kiểm tra sự khác biệt giữa các trường hợp.
3. Chỉnh sửa code trong thư mục starter/ (không chỉnh sửa fixtures để làm bài test xanh).
4. Chứng minh callback hợp lệ được chấp nhận và callback giả mạo vẫn bị từ chối.
5. Viết ngắn gọn nguyên nhân, bằng chứng và rủi ro khi triển khai production.

## Yêu cầu môi trường

- .NET SDK 8.0 trở lên, hỗ trợ target net8.0.
- PowerShell trên Windows (hoặc dùng lệnh dotnet tương đương trên macOS/Linux).
- Không cần Docker, NuGet bên ngoài, Azure hoặc secret thật.

## Chạy nhanh

~~~powershell
cd "Daily Real Engineering Lab/units/2026/10/25/UNIT-HMAC-001-payroll-webhook-signature-payload"
.\run.ps1
.\reproduce.ps1
.\verify.ps1
~~~

Trên môi trường không dùng PowerShell:

~~~sh
dotnet run --project starter/PartnerWebhookLab.csproj -- run
dotnet run --project starter/PartnerWebhookLab.csproj -- reproduce
dotnet run --project starter/PartnerWebhookLab.csproj -- verify
~~~

## Cách reproduce vấn đề

Chạy reproduce.ps1 trên code gốc. Script thành công khi chứng minh được ít nhất một callback hợp lệ bị từ chối trong khi trường hợp đối chứng hợp lệ được chấp nhận và các mẫu không hợp lệ vẫn bị từ chối. Lưu lại bảng kết quả theo từng fixture. Sau khi tự sửa, chạy verify.ps1.

Lưu ý: reproduce và verify có mục đích khác nhau; reproduce có thể không còn thành công sau khi đã sửa đúng.

## Những gì cần quan sát

- Kết quả giữa compact-valid, pretty-valid và spaced-valid có nhất quán không?
- Các fixture bị sửa nội dung hoặc có chữ ký không hợp lệ có bị từ chối không?
- Khi thay đổi cách biểu diễn payload mà không thay đổi thông tin nghiệp vụ, kết quả có thay đổi không?
- Tập hợp assertion trong verify có bảo vệ cả positive path lẫn negative path không?

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis vào workspace/my-investigation.md.
3. Thử fix trong starter/.
4. Verify trên chính starter/ đã sửa.
5. Chỉ sau đó mới xem solution/README.md.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** chỉ mở [Reference Solution](solution/README.md) sau khi đã thử sửa và chạy verify.

## Expected Results

Xem [Before](expected-results/before.md) và [After](expected-results/after.md).

## Estimated Time

50–65 phút. Mức L4 / D4; trọng tâm là phân tích contract, sửa có kiểm chứng và đánh giá rủi ro production.

# UNIT-HEADERBLEED-001 — Tenant header bị lẫn giữa hai yêu cầu billing

## Mục tiêu
Điều tra lỗi cách ly tenant trong một HTTP gateway dùng chung, tái hiện bằng test có thứ tự xác định và đưa ra bản sửa có bằng chứng.

## Bối cảnh thực tế
Một dịch vụ .NET xử lý hóa đơn của nhiều tổ chức. Hai yêu cầu hợp lệ cùng sử dụng gateway, nhưng kết quả audit cho thấy đôi khi hóa đơn của tenant A được gắn nhãn tenant B. Mô phỏng sử dụng handler nội bộ, không gọi mạng và không chứa dữ liệu thật.

## Bạn cần làm gì
Chạy starter, ghi lại request/response và thứ tự chuẩn bị hai tác vụ. Đưa ra ít nhất hai giả thuyết, xác định ranh giới dữ liệu theo request, sửa mã và kiểm chứng trường hợp đảo thứ tự.

## Yêu cầu môi trường
.NET SDK 8 hoặc mới hơn; PowerShell 7 hoặc Windows PowerShell; không cần NuGet ngoài SDK.

## Chạy nhanh
`dotnet run --project starter/Lab.csproj` từ thư mục unit.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` hoặc lệnh trên. Quan sát mã thoát và các dòng invoice/tenant. Không chạy mã trên production.

## Những gì cần quan sát
Đối chiếu hai invoice ID với tenant được yêu cầu; ghi nhận cả output lẫn exception. Phân biệt tính đúng đắn dữ liệu với lỗi kết nối mạng.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Xem gợi ý theo cấp độ](hints.md).

## Reference Solution
**Spoiler:** [Chỉ mở sau khi tự điều tra](solution/README.md).

## Expected Results
Starter: kiểm tra isolation thất bại có thể tái hiện; sau sửa: hai cặp invoice/tenant chính xác, exit code 0. Thử thêm 10 tenant, đảo thứ tự và ghi nhận bằng chứng.

## Estimated Time
40–55 phút (L3 / D4).

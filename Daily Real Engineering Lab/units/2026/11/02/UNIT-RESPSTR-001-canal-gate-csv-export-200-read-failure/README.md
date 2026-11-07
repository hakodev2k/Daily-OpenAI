# UNIT-RESPSTR-001 — Export CSV báo 200 OK nhưng đọc dữ liệu thất bại

## Mục tiêu
Điều tra một lỗi tích hợp HTTP xảy ra sau khi server đã trả về thành công. Tách biệt HTTP status, khả năng đọc payload, vòng đời tài nguyên và hành vi cleanup; chứng minh sửa lỗi bằng regression harness.

## Bối cảnh thực tế
Một dịch vụ nội bộ nhận CSV đo mực nước từ hệ thống điều khiển cửa cống Canal-17. Dashboard báo tải export thất bại dù gateway ghi nhận 200 OK. Sự cố xuất hiện ngay khi worker bắt đầu xử lý dữ liệu; đội vận hành cần giữ cơ chế streaming để tránh lưu toàn bộ file lớn trong RAM.

## Bạn cần làm gì
1. Chạy baseline và lưu lại exception, trạng thái stream và thời điểm lỗi.
2. Nêu ít nhất hai giả thuyết, chỉ ra bằng chứng loại trừ từng giả thuyết.
3. Sửa code trong starter/ExportClient.cs, giữ nguyên public API và khả năng streaming.
4. Chạy verify để kiểm tra cả đường thành công, HTTP lỗi và cleanup.
5. Chỉ xem reference sau khi đã thử fix.

## Yêu cầu môi trường
- .NET SDK 8.0+ (global.json dùng 8.0.100, cho phép roll-forward).
- PowerShell 7+ trên Windows, hoặc dùng trực tiếp dotnet CLI.
- Không cần mạng, tài khoản Azure, Docker hay NuGet package ngoài SDK.

## Chạy nhanh
Tại thư mục unit:

~~~powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
~~~

Tương đương với dotnet CLI:

~~~text
dotnet run --project harness/Probe.csproj -- reproduce
~~~

## Cách reproduce vấn đề
Chạy reproduce.ps1 trên code nguyên bản. Harness dựng fake HTTP response 200 và CSV hợp lệ, sau đó mở stream và thử đọc byte đầu tiên. Lệnh được coi là reproduce thành công khi nó quan sát đúng lỗi dự kiến và trả exit code 0. Không coi exit code 0 của reproduce là code đã được sửa.

## Những gì cần quan sát
- HTTP 200 đã được nhận nhưng thao tác đọc payload không hoàn thành.
- Loại exception và trạng thái tài nguyên ngay trước thao tác đọc.
- Sự khác nhau giữa trả về một stream và thực sự đọc stream đó.
- Đường HTTP 503 và đường caller hủy trước khi gửi request.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào workspace/my-investigation.md.
3. Thử fix trong starter/.
4. Verify bằng scripts/verify.ps1.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham chiếu](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
- Baseline: reproduce nhận diện lỗi; verify thất bại.
- Sau sửa: verify trả exit code 0; đọc đúng CSV, giữ tài nguyên sống trong lúc dùng và giải phóng khi caller kết thúc; không làm mất xử lý HTTP lỗi.
- Không phụ thuộc timing hay network.

## Estimated Time
45–60 phút (L2 Investigation, D4).

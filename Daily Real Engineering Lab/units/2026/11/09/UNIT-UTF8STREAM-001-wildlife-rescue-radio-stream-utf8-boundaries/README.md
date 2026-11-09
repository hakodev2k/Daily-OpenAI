# UNIT-UTF8STREAM-001 — Radio cứu hộ: tên địa điểm lỗi ký tự khi TCP chia gói

## Mục tiêu
Điều tra lỗi dữ liệu ký tự xuất hiện không ổn định khi ứng dụng .NET nhận bản tin vô tuyến từ gateway qua luồng byte; dùng chứng cứ ở cấp byte để phân biệt dữ liệu đầu vào hỏng với lỗi xử lý phía nhận.

## Bối cảnh thực tế
Trạm cứu hộ động vật hoang dã nhận các thông điệp UTF-8 từ gateway vô tuyến. Một số tên địa điểm có dấu hoặc emoji bị thay bằng ký tự �, nhưng chỉ ở một số lần truyền. Payload gốc có checksum hợp lệ. Ứng dụng vẫn ghi log "received".

## Bạn cần làm gì
Reproduce bằng các cách chia chunk khác nhau; ghi lại vị trí byte, kết quả từng lần chạy và ít nhất hai giả thuyết. Sửa phần xử lý trong `starter/FrameReader.cs`, không sửa fixture hoặc assertion. So sánh sau khi verify với reference solution.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell 5.1; không cần Docker, database hay network.

## Chạy nhanh
Từ thư mục unit:
```powershell
dotnet restore ./starter/RadioGateway.csproj
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` chạy fixture message giống nhau với các chunk boundaries khác nhau. Script thành công khi chứng minh được triệu chứng ở starter ban đầu; đây **không phải** test đã sửa.

## Những gì cần quan sát
So sánh hex của payload, độ dài byte, số ký tự thay thế, nội dung bản tin khi chia chunk khác nhau. Chú ý cả đường truyền có một chunk, nhiều chunk, và payload không hợp lệ. Đọc [evidence](evidence/incident.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trên starter.
4. Chạy `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp và trade-offs](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). Verify phải PASS với cả các cách chia chunk hợp lệ và fixture malformed.

## Estimated Time
45 phút.


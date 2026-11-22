# UNIT-POOLALIAS-001 — Payload cứu trợ thay đổi sau khi xếp hàng

## Mục tiêu
Điều tra tính toàn vẹn dữ liệu khi tạo payload nhị phân từ bộ nhớ dùng chung; chứng minh bằng regression test trước khi sửa.

## Bối cảnh thực tế
Dịch vụ điều phối hàng cứu trợ mã hóa mã chuyến vận chuyển thành 5 byte để đẩy vào hàng đợi. Hai chuyến được chuẩn bị liên tiếp; bản ghi của chuyến đầu có lúc bị thay đổi khi worker xử lý sau đó.

## Bạn cần làm gì
Chạy kịch bản, ghi lại byte đầu vào và byte đã xếp hàng, đề xuất ít nhất hai giả thuyết, sửa `starter/Program.cs`, rồi kiểm chứng tính độc lập của từng payload.

## Yêu cầu môi trường
.NET SDK 8.0 (xem `global.json`), PowerShell 7 hoặc Windows PowerShell. Không cần Docker, cloud hay NuGet ngoài SDK.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` từ thư mục unit. Script phải xác nhận starter đang vi phạm contract. Có thể chạy `dotnet run --project starter/DispatchLab.csproj` để xem log trực tiếp.

## Những gì cần quan sát
So sánh payload của chuyến ALPHA trước/sau khi chuẩn bị BRAVO. Phân biệt nội dung tại thời điểm encode và tại thời điểm tiêu thụ. Không dựa vào timing hay network.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md).

## Expected Results
Trước: script reproduce báo đã phát hiện lỗi; sau: `verify.ps1` trả exit code 0, hai payload giữ nguyên và độc lập.

## Estimated Time
35–45 phút.

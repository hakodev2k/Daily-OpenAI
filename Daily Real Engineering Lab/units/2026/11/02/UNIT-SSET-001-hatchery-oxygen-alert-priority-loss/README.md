# UNIT-SSET-001 — Cảnh báo oxy trại giống biến mất khỏi hàng đợi ưu tiên

## Mục tiêu
Điều tra một collection C# điều phối cảnh báo khẩn cấp, xác định vì sao cảnh báo hợp lệ không xuất hiện và bảo toàn thứ tự ưu tiên sau khi sửa.

## Bối cảnh thực tế
Một trại giống thủy sản có nhiều bể đo oxy hòa tan. Khi nhiều cảm biến báo động cùng lúc, dashboard chỉ hiển thị một phần cảnh báo mức khẩn cấp. Dịch vụ tiếp nhận vẫn báo thành công; đội vận hành có thể bỏ sót bể cần xử lý.

## Bạn cần làm gì
Reproduce, ghi dữ liệu và giả thuyết, chỉ sửa `starter/AlertQueue.cs`, rồi verify tính đầy đủ, thứ tự và trùng lặp. Không chỉnh sửa `harness/`.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell; không cần Docker, cloud, database hoặc NuGet bên ngoài SDK.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
```

## Cách reproduce vấn đề
```powershell
./scripts/reproduce.ps1
```
Exit code 0 ở reproduce nghĩa là **triệu chứng gốc đã được tái hiện**, không phải đã fix.

## Những gì cần quan sát
- Số cảnh báo đầu vào so với số lượng trong snapshot.
- Các bể bị thiếu và mức độ khẩn cấp của chúng.
- Thứ tự khi nhiều event có cùng thời điểm.
- Hành vi khi gửi lại chính xác một event.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/AlertQueue.cs`.
4. Chạy `./scripts/verify.ps1` và yêu cầu 4/4 contract PASS.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải thích và code tham chiếu](solution/README.md).

## Expected Results
Trước: chỉ 3/5 cảnh báo còn lại. Sau: 5/5 cảnh báo độc lập, thứ tự tất định, duplicate không tăng số lượng. Xem [before](expected-results/before.md) và [after](expected-results/after.md).

Nếu không reproduce, chạy `dotnet --info`, rồi `dotnet run --project starter/AlertQueue.csproj -- inspect` từ thư mục unit.

## Estimated Time
45 phút.

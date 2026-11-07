# UNIT-JLIFE-001 — Trạm quan trắc gửi JSON hợp lệ nhưng bản tin không được xuất bản

## Mục tiêu

Tự tái hiện và điều tra một lỗi vòng đời dữ liệu trong pipeline C#/.NET, thu thập exception evidence, sửa tối thiểu và kiểm chứng cả luồng thành công lẫn lỗi đầu vào.

## Bối cảnh thực tế

Dịch vụ nhận JSON từ các trạm đo chất lượng không khí, trích xuất số đo PM2.5 và chuyển sang hệ thống cảnh báo. Partner xác nhận JSON hợp lệ; nhưng một số bản tin hợp lệ vẫn không được xuất bản. Log chỉ cho thấy exception xuất hiện sau bước parse.

## Bạn cần làm gì

- Chạy baseline và ghi nhận lỗi của hai trạm có dữ liệu khác nhau.
- Xác định ranh giới giữa parse, trích xuất dữ liệu và publish.
- Đưa ra ít nhất hai giả thuyết có thể kiểm chứng.
- Sửa code trong `starter/` mà không sửa test harness hoặc thay đổi contract của `MeasurementPublisher.Publish`.
- Kiểm tra dữ liệu hợp lệ và JSON không hợp lệ; giải thích rủi ro khi giữ dữ liệu lâu hơn request.

## Yêu cầu môi trường

Windows PowerShell 5.1+ hoặc PowerShell 7; .NET 8 SDK và .NET 8 runtime. Không cần database, Docker, cloud hay NuGet package bên ngoài.

## Chạy nhanh

Tại thư mục unit:

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script phải nhận diện đúng lỗi baseline và trả exit code 0 khi reproduce thành công. Nếu thiếu SDK hoặc build lỗi, script phải thất bại thay vì báo reproduce giả.

## Những gì cần quan sát

- Cả hai JSON hợp lệ đều bị từ chối ở giai đoạn xử lý sau parse.
- Exception type và vị trí trong call stack; xác nhận không phải lỗi mạng.
- JSON không hợp lệ vẫn phải bị từ chối bằng lỗi parse thích hợp.
- Số lượng bản tin được publish thành công trước và sau sửa.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis trong [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix ở `starter/`.
4. Chạy `./verify.ps1` để kiểm tra code do bạn sửa.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ xem sau khi đã điều tra và thử sửa: [solution/README.md](solution/README.md).

## Expected Results

[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md). `reproduce.ps1` chạy baseline; `verify.ps1` chạy đúng code trong `starter/` do bạn chỉnh sửa.

## Estimated Time

30–40 phút (L1 / D3).

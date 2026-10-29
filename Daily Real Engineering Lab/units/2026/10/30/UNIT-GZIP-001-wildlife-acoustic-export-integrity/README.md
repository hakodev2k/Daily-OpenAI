# UNIT-GZIP-001 — Bản xuất dữ liệu âm thanh không vượt qua kiểm tra toàn vẹn

## Mục tiêu
Điều tra một lỗi trong pipeline tạo tệp nén .NET: phân biệt khả năng đọc dữ liệu với tính hợp lệ của artifact theo hợp đồng GZIP, thu thập bằng chứng nhị phân và chứng minh bản sửa bằng kiểm thử độc lập.

## Bối cảnh thực tế
Một nhóm nghiên cứu bảo tồn động vật hoang dã xuất dữ liệu khảo sát âm thanh hằng đêm thành GZIP để gửi cho đối tác. Công cụ nội bộ có lúc đọc lại được nội dung, nhưng cổng nhận dữ liệu từ chối một số bản xuất vì kiểm tra tính toàn vẹn không đạt. Các bản xuất nhỏ và rỗng cũng cần tuân thủ cùng một hợp đồng.

## Bạn cần làm gì
1. Chạy hệ thống mẫu và tái hiện sự khác biệt giữa payload đọc lại được và artifact đạt hợp đồng.
2. Ghi ít nhất hai giả thuyết; phân biệt lỗi nội dung, lỗi truyền tải và lỗi vòng đời tài nguyên.
3. Sửa duy nhất `starter/ArchiveExporter.cs`; giữ API và hỗ trợ payload rỗng.
4. Chạy verify, ghi bằng chứng trước/sau và đề xuất cách kiểm tra artifact khi triển khai production.

## Yêu cầu môi trường
- .NET SDK 8.x trên Windows, Linux hoặc macOS; không cần Docker, database hay dịch vụ trả phí.
- PowerShell 5.1+ hoặc PowerShell 7 để chạy các script .ps1. SDK được mô tả trong `global.json`.

## Chạy nhanh
Từ thư mục unit:
```powershell
pwsh -File ./setup.ps1
pwsh -File ./reproduce.ps1
pwsh -File ./run.ps1
```
Trên Windows PowerShell có thể dùng `powershell -ExecutionPolicy Bypass -File .\reproduce.ps1`.

## Cách reproduce vấn đề
Chạy `pwsh -File ./reproduce.ps1`. Harness kiểm tra cả payload có dữ liệu và payload rỗng; chế độ reproduce trả exit code 0 khi đã phát hiện ít nhất một artifact không đạt hợp đồng và in `header`, `footer`, `roundtrip`. Chạy `pwsh -File ./verify.ps1` trước khi sửa để quan sát kiểm thử thất bại.

## Những gì cần quan sát
- Header có đúng dạng GZIP không?
- Payload có đọc lại được không, và điều đó đã đủ để xác nhận artifact hợp lệ chưa?
- Kiểm tra tám byte cuối và kiểm tra roundtrip có cho cùng kết luận không?
- Trường hợp payload rỗng có tuân thủ cùng hợp đồng không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify cả tính đúng đắn và tính toàn vẹn.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ xem sau khi tự thử: [Solution](solution/README.md).

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)
- `reproduce.ps1`: thành công khi phát hiện lỗi của starter ban đầu.
- `verify.ps1`: chỉ thành công khi code trong `starter/` tạo artifact có header, trailer và roundtrip hợp lệ cho cả hai trường hợp.

## Estimated Time
45–60 phút (L2 investigation, D4).

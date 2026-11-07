# UNIT-EXSTACK-001 — Mất dấu điểm phát sinh lỗi khi import manifest

## Mục tiêu
Điều tra một lỗi mất thông tin chẩn đoán trong pipeline C#/.NET; chứng minh vấn đề bằng stack trace và bảo toàn hợp đồng xử lý lỗi sau khi sửa.

## Bối cảnh thực tế
Hệ thống nhập manifest container của hãng vận tải biển. Một số dòng từ đối tác có số kiện không hợp lệ. Lô hàng được từ chối đúng và audit ghi nhận lỗi, nhưng on-call không thể lần ngược vị trí lỗi ban đầu từ exception telemetry. Việc truy vết tốn thời gian dù business validation không thay đổi.

## Bạn cần làm gì
- Chạy một bản ghi hợp lệ và một bản ghi lỗi; lưu lại exception type, message, stack trace và số audit events.
- Đưa ra ít nhất hai hypothesis về nơi thông tin chẩn đoán bị mất.
- Sửa trong `starter/` mà không đổi public contract, không bỏ audit, không che lỗi.
- Chạy verify và giải thích vì sao thay đổi bảo toàn exception provenance.

## Yêu cầu môi trường
.NET SDK 8.0+ (target `net8.0`), PowerShell 7 hoặc Windows PowerShell 5.1. Không cần database, Docker, Azure hay NuGet package bên ngoài.

## Chạy nhanh
Từ thư mục unit:

```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

Trên Windows PowerShell có thể dùng `powershell -File .\scripts\reproduce.ps1`; trên PowerShell 7 dùng `pwsh -File ./scripts/reproduce.ps1`.

## Cách reproduce vấn đề
`reproduce.ps1` kiểm tra hợp đồng chẩn đoán ở trạng thái starter: một manifest hợp lệ được nhận, một manifest không hợp lệ bị từ chối, audit vẫn ghi đúng một event, nhưng dấu vết chẩn đoán không chỉ ra được tầng tạo lỗi. Script thành công (exit 0) khi **phát hiện đúng triệu chứng**; đó không phải là dấu hiệu lỗi đã được sửa.

## Những gì cần quan sát
So sánh các stack frame trong output với call chain `BatchRunner → ImportCoordinator → SourceParser`. Ghi lại tầng nào còn xuất hiện, tầng nào không; kiểm tra exception type và audit count trước/sau. Không dùng chỉ số thời gian để đánh giá lab này.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trên `starter/`.
4. Chạy `./scripts/verify.ps1` để kiểm tra code đã sửa, và chạy lại reproduce để xác nhận triệu chứng cũ biến mất.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ mở sau khi tự điều tra và thử sửa: [solution/README.md](solution/README.md).

## Expected Results
- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time
40–50 phút (L2 / D4).

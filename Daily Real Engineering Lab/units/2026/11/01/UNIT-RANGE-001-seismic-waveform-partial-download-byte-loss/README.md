# UNIT-RANGE-001 — Bản tải từng đoạn dữ liệu địa chấn bị thiếu mẫu cuối

## Mục tiêu

Điều tra sự cố integrity khi client tải một phần waveform qua HTTP. Đọc status, headers và bytes thực nhận; đưa ra giả thuyết, sửa endpoint và chứng minh các trường hợp biên không hồi quy.

## Bối cảnh thực tế

Ứng dụng khảo sát địa chất cho phép kỹ thuật viên tải các đoạn nhỏ của bản ghi địa chấn 10 byte để kiểm tra nhanh trước khi lấy tệp đầy đủ. Tải toàn bộ thành công, nhưng một số đoạn tải riêng có số byte khác với công cụ phân tích mong đợi. Dashboard vẫn ghi nhận HTTP 206.

## Bạn cần làm gì

1. Chạy bản starter, lưu lại bảng status, Content-Range và body cho từng request.
2. Đưa ra ít nhất ba giả thuyết: client ghép dữ liệu sai, server trả thiếu byte, hay metadata không đồng nhất.
3. Sửa duy nhất đường xử lý trong `starter/WaveformEndpoint.cs`; không sửa harness để làm test xanh.
4. Chạy `verify.ps1` trên starter đã sửa, bao gồm tải toàn bộ, đoạn có hai biên, một byte, đoạn mở, suffix và đoạn ngoài phạm vi.
5. Ghi lại lý do, bằng chứng và trade-offs trước khi đọc reference solution.

## Yêu cầu môi trường

- Windows PowerShell 5.1 hoặc PowerShell 7+.
- .NET SDK 8.0.x; ASP.NET Core shared framework có sẵn trong SDK.
- Không cần Docker, NuGet package ngoài, Azure hoặc kết nối Internet khi SDK đã cài.
- Harness tự mở Kestrel trên `127.0.0.1` với cổng hệ điều hành cấp phát; không gọi dịch vụ bên ngoài.

## Chạy nhanh

```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
# Sửa starter/WaveformEndpoint.cs trước khi kiểm tra:
./scripts/verify.ps1
```

## Cách reproduce vấn đề

```powershell
./scripts/reproduce.ps1
```

Harness in ra sáu request và ba giá trị cho mỗi request: `status`, `Content-Range`, `body`. `reproduce.ps1` PASS khi triệu chứng của starter ban đầu được tái hiện; nó không phải test sau sửa.

## Những gì cần quan sát

- Phản hồi `200` và `206` có cùng integrity hay không?
- Header mô tả đoạn dữ liệu nào, và body thực sự có những byte nào?
- Request chỉ một byte, đoạn đến cuối tệp và đoạn vượt độ dài cho kết quả gì?
- Tại sao HTTP success chưa đủ để kết luận dữ liệu hợp lệ?

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Giải pháp tham chiếu](solution/README.md) — chỉ xem sau khi đã tự thử sửa.

## Expected Results

- [Trước sửa](expected-results/before.md)
- [Sau sửa](expected-results/after.md)
- [Báo cáo từ client](evidence/client-report.md)
- [Workspace điều tra](workspace/my-investigation.md)

## Estimated Time

45–60 phút. L2 Investigation / D4 Production Troubleshooting. Validation: **static-review**, chưa thực thi .NET hoặc PowerShell trong quá trình phát hành.

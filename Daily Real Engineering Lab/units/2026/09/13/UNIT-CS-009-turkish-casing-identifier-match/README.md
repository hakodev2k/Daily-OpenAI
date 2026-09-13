# UNIT-CS-009 — Identifier hợp lệ nhưng lookup thất bại theo server culture

Một worker import dữ liệu nhận loại chứng từ từ hệ thống đối tác. Cấu hình cho phép `INVOICE`, còn payload gửi `invoice`. Trên một số môi trường lookup hoạt động, nhưng khi process chạy với culture khác, cùng payload lại bị báo `unknown identifier`.

## Mục tiêu

Tự reproduce lỗi phụ thuộc môi trường, thu thập evidence, xác định contract so sánh phù hợp cho identifier kỹ thuật, sửa code và verify mà không thay đổi dữ liệu đầu vào.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/13/UNIT-CS-009-turkish-casing-identifier-match"
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy starter dưới culture được cố định trong lab. Script chỉ thành công khi starter biểu hiện đúng failure mong đợi.

## Những gì cần quan sát

- Culture đang chạy.
- Hai identifier chỉ khác casing.
- Lookup trả về `Matched=False` dù business contract coi chúng là cùng identifier.
- So sánh với hành vi bạn kỳ vọng cho protocol/configuration identifier, không phải natural-language text.

## Yêu cầu của bạn

1. Reproduce trước.
2. Ghi ít nhất 2 hypothesis vào `workspace/my-investigation.md`.
3. Sửa `starter/Program.cs` để lookup ổn định giữa các culture và thể hiện rõ comparison contract.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới mở solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler — chỉ xem sau khi đã tự fix và verify.

- [Reference Solution](solution/README.md)

## Expected Results

Before: script reproduce xác nhận identifier bị từ chối trong culture mô phỏng.

After: `verify.ps1` phải exit `0`, identifier được match mà không phụ thuộc culture của process.

Nếu không reproduce được, kiểm tra `dotnet --version` và xác nhận project target `net8.0`.

## Estimated Time

35 phút.

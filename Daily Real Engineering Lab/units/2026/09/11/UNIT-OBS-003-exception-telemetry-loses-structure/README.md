# UNIT-OBS-003 — Exception Telemetry Loses Structure

## Mục tiêu

Điều tra một production-observability regression: log vẫn có text lỗi nhưng telemetry không còn nhận diện event như một exception có cấu trúc, khiến exception search, grouping và stack-based diagnosis kém hiệu quả.

## Bối cảnh thực tế

Một background worker xử lý payment vẫn ghi được message khi downstream throw exception. Dashboard log count trông bình thường, nhưng exception telemetry giảm mạnh đúng sau một refactor logging. On-call phải tìm lỗi bằng text search thay vì exception type/stack trace fields.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi evidence và ít nhất hai hypotheses vào `workspace/my-investigation.md`.
3. Điều tra cách exception được truyền vào logging API.
4. Sửa code trong `starter/` mà không thay đổi business behavior.
5. Chạy `verify.ps1` để xác nhận learner-editable path đã giữ exception như structured exception data.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Không cần Azure subscription, database hay Docker.

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/11/UNIT-OBS-003-exception-telemetry-loses-structure"
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy worker với deterministic failure và kiểm tra captured log event. Script chỉ thành công khi starter tái hiện trạng thái: message có thông tin lỗi nhưng structured exception slot không chứa exception object.

## Những gì cần quan sát

- Business operation thực sự throw exception.
- Một error-level log event vẫn được tạo.
- Message vẫn chứa dữ liệu giúp text search.
- Captured event không có exception object ở exception channel của logging abstraction.

Hãy phân biệt “message có chữ lỗi” với “telemetry pipeline nhận được exception có cấu trúc”.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi bạn đã thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Trước khi sửa

- operation throw `InvalidOperationException`
- error log được tạo
- captured structured exception là `null`

### Sau khi sửa

- error log vẫn giữ business context `OrderId`
- structured exception chứa đúng `InvalidOperationException`
- verification kết thúc với exit code `0`

Nếu script không chạy, kiểm tra `dotnet --info`, sau đó chạy `dotnet restore starter/Lab.csproj`.

## Estimated Time

30–45 phút.
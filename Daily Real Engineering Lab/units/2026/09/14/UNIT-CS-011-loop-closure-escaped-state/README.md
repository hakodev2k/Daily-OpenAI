# UNIT-CS-011 — Loop Closure Escaped State

## Mục tiêu

Điều tra một batch dispatcher tạo trước nhiều delegate cho các partition nhưng thất bại khi các delegate được chạy sau đó.

## Bối cảnh thực tế

Một background worker chuẩn bị tác vụ export theo từng region. Sau khi tách bước lập kế hoạch và bước thực thi, batch không còn xử lý được các region như trước.

## Bạn cần làm gì

- Chạy `reproduce.ps1`.
- Ghi ít nhất hai hypothesis.
- Xác định state nào thay đổi giữa lúc delegate được tạo và lúc delegate chạy.
- Sửa code trong `starter/`.
- Chạy `verify.ps1`.
- Chỉ sau đó mới xem solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa starter:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

Script reproduction build và chạy starter, sau đó xác nhận trạng thái ban đầu không hoàn tất batch đúng cách.

## Những gì cần quan sát

- Thời điểm delegate được tạo.
- Thời điểm delegate thực thi.
- Giá trị được dùng khi mỗi delegate chạy.

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

> Chỉ xem sau khi đã tự thử fix.

[Reference Solution](solution/README.md)

## Expected Results

Before:
- Batch không xử lý thành công các region đã lên kế hoạch.

After:
- `north`, `central`, `south` đều được xử lý đúng một lần.
- Không có exception.
- `verify.ps1` pass.

## Estimated Time

25–40 phút.

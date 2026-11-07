# UNIT-EF-004 — Database đã đổi nhưng API vẫn trả trạng thái cũ

## Mục tiêu

Điều tra sự khác biệt giữa trạng thái trong database và entity đang được giữ trong một `DbContext` sau một thao tác bulk update của EF Core.

## Bối cảnh thực tế

Một API nội bộ phê duyệt đơn hàng theo batch. Database audit cho thấy đơn đã chuyển sang `Approved`, nhưng response của cùng request đôi khi vẫn trả `Pending`. Request mới sau đó lại đọc đúng `Approved`.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. Ghi evidence cho cả giá trị entity trong memory và giá trị đọc trực tiếp từ database.
3. Đưa ra ít nhất hai hypothesis trước khi sửa.
4. Sửa code trong `starter/` để response cùng request phản ánh trạng thái đã persist.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell
- Không cần SQL Server/Docker; lab dùng SQLite in-memory.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Giá trị được đọc trước thao tác cập nhật.
- Số row được database xác nhận đã update.
- Giá trị entity mà code đang giữ sau update.
- Giá trị đọc lại từ database bằng một query độc lập với state đang track.

Không kết luận chỉ từ một trong hai nguồn dữ liệu.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi đã reproduce và tự thử sửa.

[Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa, output phải chứng minh database và state mà request đang giữ không đồng nhất. Sau khi sửa, cả hai đều phản ánh `Approved` mà không tạo regression cho thao tác persist.

## Estimated Time

35–50 phút.

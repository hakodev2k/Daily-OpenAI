# UNIT-EF-008 — Bulk update xong nhưng business rule vẫn đọc dữ liệu cũ

## Mục tiêu

Điều tra một EF Core workflow trong đó database đã nhận thay đổi đúng, nhưng một business rule chạy ngay sau đó trong cùng unit of work lại đưa ra quyết định dựa trên trạng thái không khớp với database.

## Bối cảnh thực tế

Một worker repricing cập nhật giá hàng loạt cho catalog. Sau bước cập nhật, worker kiểm tra sản phẩm vừa xử lý để quyết định có phát cảnh báo khi giá đạt ngưỡng hay không. Log cho thấy database có giá mới, nhưng cảnh báo đôi lúc không được phát dù batch hoàn tất thành công.

## Bạn cần làm gì

- Chạy starter và reproduce triệu chứng.
- Ghi ít nhất 2 hypothesis trước khi sửa.
- So sánh giá trị mà business rule đang đọc với giá trị thực tế trong database.
- Sửa `starter/` để business rule quan sát trạng thái phù hợp với database mà không làm thay đổi yêu cầu nghiệp vụ.
- Chạy `verify.ps1` trên chính code bạn đã sửa.
- Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Internet lần đầu để restore NuGet package

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa `starter/`:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

Script tạo SQLite database in-memory, seed một sản phẩm, chạy bước repricing rồi thực thi business rule trong cùng workflow.

## Những gì cần quan sát

- Giá trước repricing.
- Giá đọc trực tiếp từ database sau repricing.
- Giá mà business rule đang nhìn thấy.
- Việc cảnh báo ngưỡng giá có được kích hoạt hay không.

Không sửa theo cảm tính trước khi xác định vì sao hai góc nhìn dữ liệu lại khác nhau.

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

> Spoiler: chỉ xem sau khi bạn đã reproduce và tự thử fix.

[Reference Solution](solution/README.md)

## Expected Results

Before:
- Database có giá sau repricing là `110.00`.
- Business rule vẫn quan sát `100.00`.
- Alert không được kích hoạt.

After:
- Database và business rule cùng quan sát `110.00`.
- Alert được kích hoạt.
- `verify.ps1` pass trên code trong `starter/`.

## Estimated Time

30–45 phút.

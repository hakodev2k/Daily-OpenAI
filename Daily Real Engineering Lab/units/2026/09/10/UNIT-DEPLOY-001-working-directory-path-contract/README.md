# UNIT-DEPLOY-001 — Working Directory Path Contract

## Mục tiêu

Điều tra và refactor một ứng dụng .NET chạy đúng khi developer khởi động từ project folder nhưng thất bại khi được launcher/service khởi động từ working directory khác.

## Bối cảnh thực tế

Một worker tạo invoice từ template text. Trong local development, job chạy ổn. Sau khi đóng gói và chạy qua deployment script, process vẫn start thành công nhưng job đầu tiên báo không tìm thấy template. File template thực tế vẫn có trong artifact.

## Bạn cần làm gì

1. Reproduce khác biệt giữa hai cách khởi động.
2. Ghi evidence và ít nhất 2 hypothesis vào `workspace/my-investigation.md`.
3. Xác định contract về location mà code đang ngầm giả định.
4. Refactor `starter/` để artifact chạy đúng bất kể launcher chọn working directory nào.
5. Chạy `verify.ps1` và bảo toàn nội dung invoice.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Cả hai lần chạy dùng cùng source và cùng template file.
- Một cách khởi động đọc template thành công.
- Cách còn lại báo `Template not found`.
- `Environment.CurrentDirectory` khác nhau giữa hai lần chạy.
- Process vẫn start bình thường; lỗi xuất hiện khi job truy cập resource.

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

> Spoiler: chỉ xem sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before: một launch context thành công, context còn lại thất bại dù artifact có template.

After: cả hai launch context đều in `INVOICE=Invoice for ACME` và `verify.ps1` báo `LAB_VERIFY_PASS`.

## Estimated Time

35–55 phút.

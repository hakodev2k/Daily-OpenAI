# UNIT-AZURE-005 — Slot Setting Swap Boundary

## Mục tiêu

Điều tra một deployment-slot swap trong Azure App Service nơi phiên bản ứng dụng mới được đưa vào production thành công nhưng production bắt đầu ghi dữ liệu vào tài nguyên của staging.

## Bối cảnh thực tế

Một image-processing API được triển khai qua hai App Service slots: `production` và `staging`. Mỗi slot có cấu hình riêng cho storage container. Sau khi smoke test staging thành công, team thực hiện swap để đưa version mới lên production.

Ngay sau swap, request vẫn trả `200`, version mới chạy đúng, nhưng file upload từ production xuất hiện trong staging storage container.

Lab dùng local simulator cho semantics của slot swap nên không yêu cầu Azure subscription.

## Bạn cần làm gì

- Reproduce trạng thái sau swap.
- Ghi ít nhất 2 hypothesis trước khi sửa.
- Xác định cấu hình nào đang đi qua deployment boundary không đúng với intent vận hành.
- Sửa `starter/` để version ứng dụng vẫn swap nhưng production tiếp tục dùng đúng production storage target.
- Chạy `verify.ps1`.
- Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa `starter/`:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` tạo hai slot với version và configuration khác nhau, thực hiện swap, rồi xác nhận rằng application version mới đã lên production nhưng storage target của production không còn đúng.

## Những gì cần quan sát

- `PRODUCTION_VERSION`
- `PRODUCTION_ENVIRONMENT`
- `PRODUCTION_STORAGE`
- Những giá trị nào di chuyển cùng application content và những giá trị nào nên gắn với slot

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

> Reference Solution — chỉ xem sau khi đã reproduce và tự thử fix.

[Reference Solution](solution/README.md)

## Expected Results

Before:
- Production chạy application version mới.
- Request vẫn hoạt động về mặt functional.
- Production storage target bị đổi sang giá trị thuộc staging.

After:
- Production vẫn chạy application version mới.
- Production giữ đúng environment-specific storage target.
- Verification pass.

## Estimated Time

35–50 phút.

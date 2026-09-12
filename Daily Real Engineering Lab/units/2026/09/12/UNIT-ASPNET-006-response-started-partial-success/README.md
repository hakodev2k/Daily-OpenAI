# UNIT-ASPNET-006 — API trả 200 nhưng payload bị cắt giữa chừng

## Mục tiêu

Điều tra một ASP.NET Core endpoint mà client nhận HTTP `200 OK` cùng payload không hoàn chỉnh, trong khi server log lại ghi nhận exception.

## Bối cảnh thực tế

Một internal reporting API stream dữ liệu CSV để giảm memory usage. Sau khi release, monitoring xuất hiện exception rải rác nhưng một số client không retry vì response vẫn là `200 OK`. File tải về bị thiếu dòng và downstream import chỉ phát hiện lỗi muộn.

## Bạn cần làm gì

1. Chạy starter service.
2. Reproduce request lỗi bằng script.
3. Ghi lại evidence: status code, response body và server behavior.
4. Xác định tại sao global error handling không thể biến request này thành một error response đáng tin cậy.
5. Sửa code trong `starter/` để failure contract không còn phát ra `200` kèm partial CSV.
6. Chạy `verify.ps1`.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.0.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần Docker hay dịch vụ ngoài

## Chạy nhanh

```powershell
./run.ps1
```

Ở terminal khác:

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` gọi endpoint export với chế độ failure deterministic. Script PASS khi chứng minh client quan sát `200 OK` nhưng payload CSV không đầy đủ.

## Những gì cần quan sát

- HTTP status code client nhận được.
- Số dòng CSV thực tế so với contract mong đợi.
- Exception có xuất hiện sau khi một phần payload đã được gửi hay không.
- Tại thời điểm error handler chạy, response còn có thể thay đổi headers/status một cách an toàn hay không.

Không sửa bằng cách đơn giản nuốt exception hoặc chỉ thay đổi logging.

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

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Before

- Failure request trả `200 OK`.
- Body có header và một phần data rows nhưng thiếu phần còn lại.
- Server ghi nhận exception.

### After

- Failure xảy ra trước khi success response được committed, hoặc endpoint sử dụng một failure contract streaming rõ ràng.
- Reference solution chọn phương án atomic response cho report nhỏ: failure deterministic trả `500` và không trả partial CSV.
- Success request vẫn trả CSV đầy đủ.

## Estimated Time

45–60 phút.
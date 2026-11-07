# UNIT-CDN-001 — CDN trả sai ngôn ngữ dù origin đúng

## Mục tiêu
Điều tra một lỗi cache ở edge/CDN khiến response hợp lệ nhưng đôi khi thuộc sai locale.

## Bối cảnh thực tế
Một CMS public site phục vụ `/article/42` theo header `Accept-Language`. Origin luôn trả đúng nội dung, nhưng sau khi bật edge cache, người dùng `vi-VN` đôi khi nhận nội dung English và ngược lại. Không có exception; hit rate của cache lại rất tốt.

## Bạn cần làm gì
1. Reproduce triệu chứng.
2. Ghi hypothesis trước khi sửa.
3. Xác định contract nào của response chưa được phản ánh đúng tại cache boundary.
4. Sửa `starter/` mà không tắt cache toàn cục.
5. Chạy `verify.ps1`.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy hai request cùng URL nhưng khác `Accept-Language`, sau đó lặp lại theo thứ tự đảo ngược.

## Những gì cần quan sát
- status đều thành công
- origin có khả năng tạo đúng hai locale
- một response edge có thể không khớp locale request
- cache hit/miss thay đổi theo thứ tự request

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
[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results
Trước fix, cùng resource có thể trả sai locale tùy request nào làm ấm cache trước. Sau fix, từng request phải nhận đúng locale và cache vẫn tái sử dụng response an toàn.

## Estimated Time
35–50 phút.

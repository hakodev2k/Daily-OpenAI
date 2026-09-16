# UNIT-API-007 — Streaming Endpoint Looks Fast but First Byte Arrives Late

## Mục tiêu
Điều tra một ASP.NET Core export endpoint được thiết kế để stream dữ liệu nhưng client chỉ bắt đầu nhận response sau khi toàn bộ batch đã được chuẩn bị.

## Bối cảnh thực tế
Một internal reporting API xuất hàng nghìn dòng NDJSON. Dashboard production cho thấy tổng thời gian xử lý chấp nhận được, nhưng người dùng phải chờ lâu trước khi thấy dòng đầu tiên. Memory của process cũng tăng theo kích thước export.

## Bạn cần làm gì
1. Chạy starter và ghi lại thứ tự các mốc `PRODUCED` và `CLIENT_RECEIVED`.
2. Ghi ít nhất hai hypothesis giải thích vì sao API có contract streaming nhưng client vẫn chờ.
3. Xác định boundary nào đang giữ dữ liệu lâu hơn cần thiết.
4. Sửa code trong `starter/` để client có thể quan sát dữ liệu tăng dần mà vẫn giữ đúng thứ tự và nội dung.
5. Chạy `verify.ps1` trên code bạn đã sửa.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy starter với dataset cố định và kiểm tra rằng client không quan sát được record đầu tiên cho đến khi producer đã hoàn thành toàn bộ batch.

## Những gì cần quan sát
- Thứ tự timestamp/logical markers giữa producer và consumer.
- Khi nào record đầu tiên trở nên observable ở client.
- Peak buffered item count được starter báo cáo.
- Nội dung và thứ tự record phải giữ nguyên sau khi sửa.

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
[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
**Before:** producer hoàn tất batch trước khi client nhận record đầu tiên; số item được giữ đồng thời tăng theo dataset.

**After:** client quan sát record đầu tiên khi producer vẫn còn đang tạo các record sau; output vẫn đủ và đúng thứ tự; không cần giữ toàn bộ dataset trước khi bắt đầu delivery.

## Estimated Time
35–55 phút.
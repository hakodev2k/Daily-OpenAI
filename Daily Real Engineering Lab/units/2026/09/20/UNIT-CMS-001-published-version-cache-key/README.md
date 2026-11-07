# UNIT-CMS-001 — Published Content Version Cache Boundary

## Mục tiêu
Điều tra một lỗi stale content trong luồng publish CMS và xác định cache contract phù hợp.

## Bối cảnh thực tế
Marketing publish bản mới của một landing page. CMS xác nhận publish thành công nhưng public delivery vẫn trả nội dung cũ trong một khoảng thời gian. Restart application làm nội dung mới xuất hiện ngay.

## Bạn cần làm gì
Reproduce, ghi evidence/hypothesis, sửa starter, chạy verify, rồi so sánh reference solution.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell. Lab dùng local simulator, không yêu cầu Optimizely license hay external service.

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Script chạy một request, mô phỏng publish version tiếp theo, rồi request lại cùng content item.

## Những gì cần quan sát
Content version ở source, cache hit/miss, nội dung response trước và sau publish, và thời điểm cache entry thay đổi.

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
[Spoiler](solution/README.md)

## Expected Results
Before: publish thành công nhưng request tiếp theo vẫn thấy version cũ. After: request sau publish phản ánh version mới mà không cần restart.

Nếu không reproduce được, chạy `dotnet run --project starter -- reproduce` và kiểm tra output version/cache.

## Estimated Time
40 phút.
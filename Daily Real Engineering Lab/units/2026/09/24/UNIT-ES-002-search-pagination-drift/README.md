# UNIT-ES-002 — Search Pagination Drift Under Concurrent Updates

## Mục tiêu
Điều tra một lỗi production trong API tìm kiếm khi người dùng duyệt nhiều trang trong lúc dữ liệu vẫn thay đổi.

## Bối cảnh thực tế
Support portal trả 10 ticket mỗi trang, sắp xếp theo thời điểm cập nhật giảm dần. Khi traffic ghi tăng, QA báo một ticket xuất hiện ở cả page 1 và page 2, trong khi ticket khác biến mất khỏi chuỗi kết quả. Mỗi request riêng lẻ đều trả HTTP 200 và đủ 10 phần tử.

## Bạn cần làm gì
Reproduce hiện tượng, ghi lại evidence và hypothesis, xác định contract pagination phù hợp, sửa code trong `starter/`, rồi chạy verification.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy một sequence gồm page đầu, một thay đổi dữ liệu giữa hai request, rồi page tiếp theo. Script thất bại khi chuỗi duyệt không còn ổn định.

## Những gì cần quan sát
- từng page riêng lẻ vẫn hợp lệ về kích thước;
- tập ID quan sát qua hai page có thể không còn đúng như snapshot ban đầu;
- log hiển thị thứ tự item ở từng request và mutation xảy ra giữa hai request.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results
Trước fix, sequence có duplicate/missing ID. Sau fix, một pagination session phải duyệt ổn định cùng logical result set và có deterministic ordering.

## Estimated Time
60 phút.
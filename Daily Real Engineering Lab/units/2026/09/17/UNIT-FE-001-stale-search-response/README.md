# UNIT-FE-001 — Search UI đôi lúc hiển thị kết quả của query cũ

## Mục tiêu
Điều tra một race condition phía frontend khi nhiều asynchronous search request hoàn thành không theo thứ tự gửi đi.

## Bối cảnh thực tế
Một trang quản trị có ô tìm kiếm tức thời. Khi người dùng gõ nhanh, network latency của từng request khác nhau. QA báo rằng input đang hiển thị query mới nhưng danh sách kết quả đôi lúc thuộc query trước đó.

## Bạn cần làm gì
1. Chạy starter và reproduce symptom.
2. Ghi hypothesis trước khi sửa.
3. Theo dõi thứ tự request bắt đầu, hoàn thành và state được cập nhật.
4. Sửa code để UI chỉ commit kết quả còn hợp lệ với search intent hiện tại.
5. Chạy `verify.ps1`.
6. Sau đó so sánh với reference solution.

## Yêu cầu môi trường
- Node.js 20+
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Query cuối cùng mà user nhập.
- Thứ tự completion của các asynchronous operation.
- Giá trị cuối cùng được commit vào UI state.
- Sau fix, query mới nhất phải luôn sở hữu kết quả hiển thị.

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
[Reference Solution — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
**Before:** state cuối có thể chứa kết quả của query cũ.

**After:** completion muộn của request cũ không thể ghi đè kết quả tương ứng với search intent mới nhất.

## Estimated Time
30–45 phút.
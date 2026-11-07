# UNIT-EF-010 — Hai DbContext, một business operation, hai object graph

## Mục tiêu
Điều tra một production incident nơi API trả về thành công nhưng một thay đổi quan hệ quan trọng không xuất hiện trong trạng thái cuối cùng.

## Bối cảnh thực tế
Một internal API xử lý credit adjustment cho customer. Request cần đọc customer, đọc account liên quan, cập nhật trạng thái và lưu lại. Log cho thấy các bước đều chạy, không có exception, nhưng một phần thay đổi biến mất sau request. Sự cố chỉ xuất hiện sau khi service được tách thành các repository nhỏ hơn.

## Bạn cần làm gì
Reproduce triệu chứng, thu thập evidence về object identity và lifecycle của persistence objects, ghi ít nhất 2 hypotheses, sửa starter để toàn bộ business operation có kết quả nhất quán, rồi verify.

## Yêu cầu môi trường
- .NET SDK 8.0.x
- PowerShell
- Không cần database hoặc cloud service bên ngoài

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Lab dùng một persistence simulator nhỏ để biểu diễn semantics cần điều tra. Script phải chứng minh request hoàn tất nhưng persisted relationship state không đúng như business expectation.

## Những gì cần quan sát
- Các instance ID được log ở từng repository boundary.
- Trạng thái trước và sau từng bước.
- Object nào thực sự được persistence boundary cuối cùng quan sát.
- Không suy luận root cause chỉ từ việc request không có exception.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
Trước fix, operation kết thúc nhưng persisted account relationship không phản ánh thay đổi mong muốn. Sau fix, `verify.ps1` phải báo `VERIFY_PASS` và các bước trong cùng operation phải quan sát một identity-consistent object graph.

## Estimated Time
55 phút.

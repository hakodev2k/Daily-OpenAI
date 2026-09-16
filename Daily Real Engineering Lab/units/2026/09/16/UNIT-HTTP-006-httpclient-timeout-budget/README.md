# UNIT-HTTP-006 — Request deadline bị vượt qua chuỗi HTTP dependency

## Mục tiêu
Điều tra cách một API quản lý time budget khi gọi nhiều downstream HTTP operations và sửa behavior để request không tiếp tục tiêu thụ tài nguyên sau khi deadline nghiệp vụ đã hết.

## Bối cảnh thực tế
Một endpoint tổng hợp thông tin giao hàng gọi tuần tự hai dependency nội bộ. Mỗi dependency thường phản hồi nhanh, nhưng khi cả hai cùng chậm vừa phải, endpoint trả kết quả muộn hơn SLO mặc dù từng HTTP call riêng lẻ vẫn chưa chạm timeout cấu hình của nó.

## Bạn cần làm gì
Reproduce tình huống, ghi hypothesis, xác định time budget thực tế của toàn request, chỉnh code trong `starter/`, rồi chạy verification để chứng minh cả correctness lẫn deadline behavior.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ khuyến nghị
- Không cần external service hoặc cloud account

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy starter với downstream simulator deterministic. Hai dependency đều hoàn thành riêng lẻ, nhưng tổng thời gian request vượt budget mà business contract cho phép.

## Những gì cần quan sát
- Tổng elapsed time của operation.
- Thời điểm mỗi dependency bắt đầu/kết thúc.
- Request có tiếp tục downstream work sau khi budget tổng thể đáng lẽ đã hết hay không.
- Phân biệt per-call timeout với end-to-end request deadline.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Reference Solution](solution/README.md) — spoiler, chỉ xem sau khi đã tự thử fix.

## Expected Results
Trước fix, aggregate operation có thể vượt end-to-end budget dù từng call riêng lẻ hợp lệ. Sau fix, learner-editable path phải dừng trong budget cho phép và vẫn trả đúng kết quả khi dependencies phản hồi đủ nhanh.

## Estimated Time
35–55 phút.
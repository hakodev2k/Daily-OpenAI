# UNIT-LEGACY-001 — Time Boundary Refactor

## Mục tiêu
Refactor một business rule cũ theo hướng testable và replayable mà không thay đổi behavior công khai.

## Bối cảnh thực tế
Subscription renewal service có rule xác định subscription đã hết hạn. Test suite đôi khi fail gần ranh giới thời gian, còn production support không thể tái hiện chính xác quyết định đã xảy ra tại một timestamp cụ thể.

## Bạn cần làm gì
1. Chạy starter tests nhiều lần và đọc các boundary cases.
2. Ghi hypothesis về dependency nào khiến kết quả khó kiểm soát.
3. Refactor với thay đổi nhỏ nhất có thể, giữ public behavior.
4. Bổ sung regression tests cho các instant trước, đúng tại, và sau expiration boundary.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Starter chứa test mô phỏng một production incident bằng cách so sánh kết quả business rule với một instant đã ghi nhận. Hãy chạy test và quan sát vì sao cùng dữ liệu nghiệp vụ nhưng không thể buộc code đánh giá tại instant đó.

## Những gì cần quan sát
- Business input nào được truyền rõ ràng và dependency nào bị lấy ngầm từ process/runtime.
- Test có đang kiểm soát đầy đủ mọi input quyết định kết quả không.
- Refactor có làm thay đổi API hoặc semantics không cần thiết không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử refactor.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Reference Solution](solution/README.md)

## Expected Results
Before: incident replay test không thể điều khiển instant dùng bởi business rule. After: tests xác định được behavior tại các boundary instant mà không sleep/chờ clock thật.

## Estimated Time
45–60 phút.
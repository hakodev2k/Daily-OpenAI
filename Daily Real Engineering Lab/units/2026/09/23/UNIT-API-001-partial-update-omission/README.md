# UNIT-API-001 — Partial Update Omission Boundary

## Mục tiêu
Điều tra một API cập nhật một phần resource nhưng làm thay đổi thêm field mà client không gửi.

## Bối cảnh thực tế
Merchant Portal cho phép cập nhật từng phần notification settings. Một client chỉ đổi display name, nhưng sau request một setting đang bật lại bị tắt. Request vẫn trả success và không có exception.

## Bạn cần làm gì
Reproduce hành vi, ghi hypothesis, xác định boundary làm mất ý nghĩa của request, sửa starter và verify rằng field không được gửi sẽ được giữ nguyên trong khi client vẫn có thể chủ động gửi giá trị false.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+

## Chạy nhanh
Chạy ./run.ps1 rồi ./reproduce.ps1.

## Cách reproduce vấn đề
Chạy reproduce.ps1. Script kiểm tra một update chỉ chứa thay đổi tên và mong đợi notification setting cũ không đổi.

## Những gì cần quan sát
- State trước request
- Payload logic mà client muốn gửi
- State sau update
- Có exception hay validation error nào xuất hiện không

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter/.
4. Chạy verify.ps1.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results
Before: update một field có thể làm thay đổi field khác ngoài ý định client.
After: field bị omit được giữ nguyên; explicit false vẫn cập nhật thành false.

## Estimated Time
40 phút.

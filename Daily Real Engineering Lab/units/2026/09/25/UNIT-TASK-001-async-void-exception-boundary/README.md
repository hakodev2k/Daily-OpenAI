# UNIT-TASK-001 — Async Callback Escapes the Caller Contract

## Mục tiêu
Điều tra một import flow báo `completed` trước khi callback kết thúc và caller không bắt được lỗi phát sinh sau `await`.

## Bối cảnh thực tế
Một helper cũ nhận `Action<T>`. Developer truyền `async` lambda để gọi I/O.

## Bạn cần làm gì
Reproduce, ghi evidence và hypotheses, sửa callback contract để completion/error thuộc về caller, rồi verify.

## Yêu cầu môi trường
.NET SDK 8.x.

## Chạy nhanh
`dotnet run --project starter/Lab.csproj`

## Cách reproduce vấn đề
Quan sát thứ tự log và đường truyền exception.

## Những gì cần quan sát
Kiểu delegate thực tế, thời điểm method caller hoàn tất và ai sở hữu asynchronous operation.

## Quy tắc làm lab
Reproduce → evidence → hypotheses → fix → verify → solution.

## Hints
[h1](hints/hint-01.md) · [h2](hints/hint-02.md) · [h3](hints/hint-03.md)

## Reference Solution
[solution](solution/README.md)

## Expected Results
[before](expected-results/before.md) · [after](expected-results/after.md)

## Estimated Time
45 phút.
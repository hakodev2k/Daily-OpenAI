# UNIT-DESIGN-001 — Time Is a Hidden Dependency

## Mục tiêu
Refactor một renewal rule để thời gian trở thành dependency rõ ràng và test boundary deterministic.

## Bối cảnh thực tế
Service quyết định subscription có được renew hay không dựa trực tiếp vào wall clock tại nhiều điểm.

## Bạn cần làm gì
Reproduce boundary behavior, ghi evidence/hypotheses, refactor mà không đổi business rule, rồi verify.

## Yêu cầu môi trường
.NET SDK 8.x.

## Chạy nhanh
`dotnet run --project starter/Lab.csproj`

## Cách reproduce vấn đề
Đọc starter, xác định mọi clock read và thử reasoning ở expiration boundary.

## Những gì cần quan sát
Dependency nào đang bị ẩn, boundary nào cần một instant thống nhất, và API nào giúp test kiểm soát thời gian.

## Quy tắc làm lab
Reproduce → evidence → hypotheses → refactor → verify → solution.

## Hints
[h1](hints/hint-01.md) · [h2](hints/hint-02.md) · [h3](hints/hint-03.md)

## Reference Solution
[solution](solution/README.md)

## Estimated Time
45 phút.
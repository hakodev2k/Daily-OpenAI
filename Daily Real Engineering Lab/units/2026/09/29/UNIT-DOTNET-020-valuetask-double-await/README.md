# UNIT-DOTNET-020 — Async result chỉ lỗi ở slow path

## Mục tiêu

Điều tra một lỗi async khi cùng một kết quả bất đồng bộ được sử dụng lại ở hai bước xử lý: fast path chạy được nhưng slow path fail.

## Bối cảnh thực tế

Pricing worker lấy exchange rate để tính giá và enrich telemetry. Local thường chạy fast path; production đôi lúc đi slow path và request fail sau khi business calculation đã có kết quả.

## Bạn cần làm gì

1. Reproduce cả hai path.
2. Ghi evidence và ít nhất hai hypothesis.
3. Sửa `starter/PricingProcessor.cs`.
4. Giữ nguyên business calculation và telemetry.
5. Chạy `verify.ps1`.
6. Sau đó mới xem solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

~~~powershell
./reproduce.ps1
./verify.ps1
~~~

## Cách reproduce vấn đề

Chạy `reproduce.ps1`, so sánh output FAST và SLOW.

## Những gì cần quan sát

- một asynchronous result được consumer sử dụng bao nhiêu lần
- fast và slow path khác nhau ở observable behavior nào
- failure xảy ra trước hay sau business calculation

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

[Spoiler](solution/README.md)

## Expected Results

- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time

30–45 phút.

# UNIT-TEST-002 — Tests Pass Alone, Fail Together

## Mục tiêu

Điều tra một test suite .NET có hành vi ổn định khi chạy từng test riêng lẻ nhưng thất bại khi chạy song song, thu thập bằng chứng và sửa cả test design lẫn production dependency boundary để test độc lập với nhau.

## Bối cảnh thực tế

Một dịch vụ subscription renewal đang chuyển dần giữa hai provider. CI bắt đầu báo đỏ không ổn định sau khi đội tăng mức parallelism của test runner. Developer chạy riêng từng failing test trên máy cá nhân thì test đều xanh, vì vậy lỗi bị quy cho CI hoặc xUnit.

Business impact: pipeline bị rerun nhiều lần, regression thật khó phân biệt với test noise, và team mất niềm tin vào test suite.

## Bạn cần làm gì

1. Chạy từng test riêng và ghi lại kết quả.
2. Reproduce khi hai test chạy cùng nhau dưới controlled parallel gate.
3. Xác định state/dependency nào có thể bị chia sẻ ngoài ý muốn giữa hai test.
4. Sửa `starter/` theo cách giữ được parallel test execution.
5. Chạy `verify.ps1` cho tới khi toàn bộ test ổn định.
6. Chỉ sau đó mới so sánh với reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7 hoặc Windows PowerShell
- Không cần database, Docker hay cloud service

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

Script sẽ:

- restore/build project
- chạy từng test độc lập
- sau đó bật controlled parallel gate và chạy cả suite

Một reproduction hợp lệ phải cho thấy hai trạng thái chạy có kết quả khác nhau.

## Những gì cần quan sát

- test nào pass khi chạy riêng
- test nào fail khi chạy cùng suite
- expected provider của từng test
- actual provider xuất hiện trong assertion failure
- mọi mutable state có lifetime dài hơn một test case

Không giả định test runner là nguyên nhân trước khi có evidence.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

> **Spoiler:** chỉ mở sau khi bạn đã tự reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa:

- mỗi test có thể pass khi chạy độc lập
- suite chạy dưới controlled parallel gate phải fail ít nhất một assertion
- failure thể hiện cross-test interference, không phải timing performance

Sau khi sửa:

- cả hai test vẫn chạy song song
- suite pass ổn định
- mỗi test sở hữu configuration/dependency riêng
- không cần retry hoặc disable parallelism để che failure

## Estimated Time

30–50 phút.

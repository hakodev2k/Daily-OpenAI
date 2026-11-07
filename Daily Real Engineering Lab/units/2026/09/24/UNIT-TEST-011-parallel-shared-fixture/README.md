# UNIT-TEST-011 — Parallel Tests and Shared Fixture State

## Mục tiêu
Điều tra một integration-test suite ổn định khi chạy từng test nhưng thỉnh thoảng fail khi chạy song song.

## Bối cảnh thực tế
Team inventory vừa bật parallel execution để rút ngắn CI. Hai test kiểm tra reservation độc lập đều pass khi chạy riêng. Khi suite chạy song song, một test đôi lúc quan sát dữ liệu không còn đúng với setup của chính nó.

## Bạn cần làm gì
Reproduce, ghi hypothesis, xác định evidence liên quan, sửa `starter/` để các test độc lập thật sự và verify mà không tắt parallel execution.

## Yêu cầu môi trường
.NET SDK 8.x.

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script chạy nhiều vòng song song để làm triệu chứng dễ quan sát hơn.

## Những gì cần quan sát
- Test nào fail và test nào vẫn pass.
- Failure có biến mất khi chạy từng test riêng không.
- Timeline các thao tác setup/read giữa hai test.
- Functional expectation của từng test phải giữ nguyên sau khi sửa.

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
[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
Before: suite có thể fail khi hai test overlap. After: nhiều vòng parallel đều ổn định và vẫn kiểm tra cùng behavior.

## Estimated Time
45 phút.
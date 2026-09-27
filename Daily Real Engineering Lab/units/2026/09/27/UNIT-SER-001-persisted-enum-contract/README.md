# UNIT-SER-001 — Persisted Enum Contract Breaks After Refactor

## Mục tiêu
Điều tra và refactor một serialization boundary bị regression sau khi model domain được thay đổi, đồng thời giữ backward compatibility với dữ liệu đã lưu.

## Bối cảnh thực tế
Một order workflow service đã chạy ổn định nhiều tháng và lưu snapshot JSON để resume processing. Sau một deployment refactor model trạng thái, các order cũ vẫn deserialize thành công nhưng một số order quay về sai business state. Dữ liệu mới không có exception và pipeline vẫn chạy.

## Bạn cần làm gì
1. Reproduce regression bằng historical fixture có sẵn.
2. Thu evidence từ payload, deserialized value và expected business meaning.
3. Ghi ít nhất hai hypotheses trước khi sửa.
4. Refactor starter để dữ liệu lịch sử vẫn được hiểu đúng, đồng thời contract tương lai rõ ràng hơn.
5. Chạy `verify.ps1` và sau đó so sánh với reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy starter với historical JSON fixture. Chương trình sẽ in raw payload, trạng thái đã deserialize và business expectation của fixture.

## Những gì cần quan sát
- Payload có deserialize thành công hay không.
- Giá trị business state sau deserialize.
- Điều gì thay đổi giữa persisted representation và model hiện tại.
- Một fix có giữ được cả dữ liệu lịch sử lẫn khả năng evolution về sau hay không.

Không xem việc “deserialize không throw” là bằng chứng contract vẫn đúng.

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
[Spoiler — chỉ xem sau khi tự điều tra](solution/README.md)

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
55 phút.
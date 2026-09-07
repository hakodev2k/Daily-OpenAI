# Reference Solution — Spoiler

Root cause là function `date(CreatedUtc)` được áp lên indexed column. Query vẫn đúng về mặt kết quả nhưng predicate không còn ở dạng range trực tiếp trên key mà B-tree index có thể search hiệu quả.

Một fix phù hợp là half-open range:

```csharp
var sql = "SELECT COUNT(*) FROM Orders WHERE CreatedUtc >= $start AND CreatedUtc < $end";
```

với `$start = "2026-09-07 00:00:00"` và `$end = "2026-09-08 00:00:00"`.

Half-open interval tránh các lỗi kiểu `23:59:59.999...`, giữ semantics rõ ràng và cho optimizer range-search trên index.

## Trade-offs

- Với production database, kiểu dữ liệu thời gian nên là native temporal type khi engine hỗ trợ; lab dùng SQLite text ISO timestamp để tự chứa và deterministic.
- Timezone/calendar-day semantics phải được định nghĩa rõ. “Ngày theo UTC” và “ngày theo timezone business” không giống nhau.
- Expression index có thể là lựa chọn trong một số engine, nhưng không nên là phản xạ đầu tiên khi predicate có thể viết sargable đơn giản hơn.
- Luôn so execution plan và đo workload thật thay vì chỉ dựa vào việc “có index”.

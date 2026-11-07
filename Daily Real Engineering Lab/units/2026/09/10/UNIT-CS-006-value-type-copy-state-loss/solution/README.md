# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Mỗi iteration gọi `Increment()` và local log cho thấy `ObservedCount=1`, nhưng sau ba iteration, dictionary vẫn trả về `PersistedCount=0`.

## 2. Evidence

- Key `tenant-a` luôn tồn tại.
- Không có concurrency trong reproduction.
- Local variable sau `Increment()` có giá trị đã thay đổi.
- Lần `TryGetValue` kế tiếp lại bắt đầu từ state cũ trong dictionary.

## 3. Root cause

`RateCounter` là `struct`, tức value type. `Dictionary<TKey,TValue>.TryGetValue` copy value ra biến `counter`. `counter.Increment()` chỉ mutate bản copy local đó. Dictionary không tự nhận lại bản copy đã thay đổi.

Vì dictionary vẫn giữ `RateCounter(0)`, mỗi iteration lại copy từ giá trị `0`, local tăng thành `1`, rồi bản local bị bỏ đi.

## 4. Why the fix works

Reference solution ghi value đã mutate trở lại dictionary:

```csharp
counter.Increment();
counters["tenant-a"] = counter;
```

Bây giờ state lưu trữ trở thành `1`, rồi `2`, rồi `3` qua các iteration.

## 5. How to verify

Áp dụng fix vào `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Pass condition:

- `Iteration=3 ObservedCount=3`
- `PersistedCount=3`

## 6. Alternative fixes

### Dùng reference type

Nếu `RateCounter` thực sự đại diện cho một stateful object có identity và được mutate tại chỗ, đổi nó thành `class` có thể phù hợp hơn. Khi đó dictionary giữ reference đến cùng object.

### Dùng immutable value type

Một lựa chọn rõ semantics hơn là dùng immutable `readonly record struct` rồi tạo value mới mỗi lần update và gán lại vào dictionary. Cách này làm write-back trở thành một phần rõ ràng của model.

## 7. Wrong / tempting fixes

### Chỉ gọi `Increment()` nhiều lần hơn

Không giải quyết boundary giữa local copy và persisted value; bạn chỉ mutate bản copy nhiều hơn.

### Dùng `ref` hoặc low-level API ngay lập tức

Có các API nâng cao cho phép truy cập value trong collection theo reference trong một số trường hợp, nhưng đây thường là complexity không cần thiết cho quota state đơn giản và dễ tạo coupling với implementation detail.

### Thêm lock

Không có race trong reproduction. Lock không thay đổi copy semantics của value type.

## 8. Production implications

Bug dạng này nguy hiểm vì không có exception và log cục bộ có thể trông hợp lệ. Với counters, quotas, retry state hoặc aggregate state, hệ thống có thể âm thầm mất update dù code path dường như đã mutate state.

## 9. Trade-offs

- `struct` phù hợp cho value nhỏ, có semantics theo giá trị và thường tốt nhất khi immutable.
- `class` phù hợp hơn khi identity và shared mutable state là một phần tự nhiên của domain model.
- Nếu dùng mutable value type, code cần cực kỳ rõ về copy boundary và write-back.

## 10. What a Senior engineer should notice

Đừng chỉ hỏi “line nào sai”; hãy xác định ownership và lifetime của state. Khi một collection trả về value type, cần biết bạn đang giữ state gốc hay một bản copy. Senior engineer cũng nên nhận ra rằng mutable structs thường làm code reasoning khó hơn và cần được dùng có chủ đích.

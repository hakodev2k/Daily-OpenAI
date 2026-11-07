# Reference Solution — inspect only after reproducing and attempting your own fix

## 1. Symptoms

Batch tạo ba callback nhưng khi các callback chạy sau khi vòng lặp kết thúc, chúng không còn quan sát state của từng iteration. Starter vì vậy có thể ném `IndexOutOfRangeException` thay vì trả kết quả cho `alpha`, `bravo`, `charlie`.

## 2. Evidence

- Có đúng ba callback được tạo.
- Collection `tenants` không bị mutate.
- Failure xảy ra lúc callback dereference `tenants[i]`, không phải lúc callback được tạo.
- Sau vòng `for`, biến `i` đã có giá trị bằng `tenants.Length`.

## 3. Root cause

Lambda capture biến, không capture giá trị hiện tại của biểu thức. Các lambda trong starter cùng tham chiếu tới loop variable `i`. Khi chúng được invoke sau vòng lặp, `i` đã tiến tới giá trị cuối cùng, nên `tenants[i]` dùng index không còn hợp lệ.

`await` không phải nguyên nhân gốc; asynchronous scheduling chỉ làm khoảng cách giữa lúc tạo delegate và lúc delegate đọc captured state trở nên rõ hơn.

## 4. Why the fix works

Tạo một local value riêng cho từng iteration trước khi tạo delegate:

```csharp
var tenant = tenants[i];
```

Mỗi lambda sau đó capture biến `tenant` của iteration tương ứng. State mà work item cần dùng không còn phụ thuộc vào loop variable tiếp tục thay đổi.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Verification chạy trực tiếp `starter/` mà bạn đã sửa và yêu cầu output chứa chính xác:

```text
alpha,bravo,charlie
```

## 6. Alternative fixes

- Dùng `foreach (var tenant in tenants)` và capture `tenant`; với C# hiện đại, iteration variable của `foreach` có semantics phù hợp cho trường hợp này.
- Truyền tenant thành argument cho method tạo work item thay vì để lambda phụ thuộc vào mutable outer state.
- Nếu không cần deferred delegate, tạo task bằng method nhận trực tiếp tenant value.

## 7. Wrong / tempting fixes

- Thêm `Task.Delay` để "đổi timing": chỉ che symptom và làm failure nondeterministic hơn.
- Chạy tuần tự toàn bộ batch: có thể tránh interleaving nhưng đánh mất concurrency mà không sửa state-capture contract.
- Bắt `IndexOutOfRangeException` rồi retry: retry vẫn dùng cùng logic sai.
- Copy toàn bộ `tenants` collection: collection không phải state thay đổi; vấn đề nằm ở captured loop variable.

## 8. Production implications

Closure bug không phải lúc nào cũng ném exception. Nếu captured state vẫn trỏ tới một value hợp lệ, hệ thống có thể xử lý nhầm customer/tenant/job mà không crash — nguy hiểm hơn vì dữ liệu vẫn có vẻ hợp lệ.

## 9. Trade-offs

Capture một immutable/stable per-iteration value là fix nhỏ và rõ nhất. Refactor sang method có parameter thường dễ test và ít phụ thuộc closure hơn, nhưng có thể dài hơn. Serialize batch chỉ hợp lý nếu business constraint thực sự yêu cầu serialization, không phải để né closure bug.

## 10. What a Senior engineer should notice

Senior engineer nên phân biệt ba thời điểm: tạo delegate, schedule/invoke delegate, và đọc captured state. Khi review fan-out/concurrent code, hãy nhìn không chỉ `await`/`Task`, mà cả mutable outer variables, ownership và lifetime của state đi qua asynchronous boundary.
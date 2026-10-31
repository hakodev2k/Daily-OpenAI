# Reference Solution — SPOILER

**Chỉ xem sau khi reproduce và tự sửa.** Đây là lời giải tham khảo; hai file `GatePolicy.cs` và `GatePolicyTests.cs` nằm tách biệt, không được `verify.ps1` dùng trực tiếp.

## 1. Symptoms
CI báo xanh trong khi acceptance probe cho thấy thẻ hết hạn được chấp nhận.

## 2. Evidence
Test `Expired_badge_must_be_rejected` khai báo `void` và gọi asynchronous assertion nhưng không quan sát kết quả. Independent probe trực tiếp `await ValidateAsync` và nhận success thay vì exception.

## 3. Root cause
Hai lỗi độc lập kết hợp: policy trả về `Task.CompletedTask` cho thẻ hết hạn; xUnit `Assert.ThrowsAsync` trả về `Task` nhưng test không `await`, khiến assertion chạy mà kết quả lỗi không được test runner chờ/ghi nhận.

## 4. Why the fix works
Đổi test thành `async Task` và `await Assert.ThrowsAsync<AccessDeniedException>(...)`. Policy trả về `Task.FromException(new AccessDeniedException("expired"))` tại nhánh hết hạn. Fault-injection hook cố ý giữ nguyên để kiểm tra khả năng bắt regression.

## 5. How to verify
Chép nội dung `solution/GatePolicy.cs` vào `starter/RailYard.Access/GatePolicy.cs`; chép `solution/GatePolicyTests.cs` vào `starter/RailYard.Access.Tests/GatePolicyTests.cs`; chạy `./verify.ps1`. Script kiểm tra behavior và yêu cầu test suite đỏ khi `LAB_FAULT_INJECTION=accept-expired`.

## 6. Alternative fixes
Có thể triển khai `async Task ValidateAsync` và `throw` trực tiếp, hoặc trả `Task.FromException` như mẫu. Có thể dùng `Assert.ThrowsAsync` hoặc assertion library tương đương, nhưng phải `await`/return Task cho runner.

## 7. Wrong / tempting fixes
- Chỉ sửa policy: regression test vẫn false green; mutation gate phát hiện.
- Chỉ sửa test: test đỏ, policy production vẫn sai.
- Thêm `Thread.Sleep` hay `Task.Delay`: không bảo đảm test quan sát assertion.
- `async void` test: exception propagation và lifecycle khó kiểm soát; dùng `async Task`.
- Xóa fault-injection hook để làm mutation gate xanh: loại bỏ phép chứng minh độ nhạy của test.

## 8. Production implications
False-green CI ở access control có thể khiến triển khai policy lỗi. Nên có negative-path contract tests, mutation tests ở các invariant quan trọng và code review cảnh báo unawaited tasks (CS4014/analyzers).

## 9. Trade-offs
Mutation gate tăng thời gian CI và yêu cầu bảo trì fault hook; nên áp dụng có chọn lọc ở business invariants có rủi ro cao. Independent acceptance probe giảm nguy cơ test cùng lặp lại assumption sai của implementation.

## 10. Senior engineer should notice
Phải tách **business correctness** và **test effectiveness**. Test suite pass không phải chứng cứ đủ; cần chứng minh test chuyển đỏ khi hành vi bị phá, kiểm tra boundary đúng thời điểm hết hạn, và duy trì kiểm soát fault injection chỉ trong lab/test environment.

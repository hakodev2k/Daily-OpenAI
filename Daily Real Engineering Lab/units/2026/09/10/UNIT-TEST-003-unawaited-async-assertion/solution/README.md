# Reference Solution

> Chỉ xem sau khi đã reproduce và thử fix.

## Symptoms
Test suite báo PASS cho failure-path test.

## Evidence
`Assert.ThrowsAsync<T>` trả về một `Task`. Starter test method trả về `void` và không `await` task đó, vì vậy runner có thể coi test đã hoàn tất trước khi assertion bất đồng bộ hoàn tất.

## Root cause
Async assertion được tạo nhưng completion/failure của nó không trở thành một phần lifecycle của test method.

## Why the fix works
Đổi test thành `async Task` và `await Assert.ThrowsAsync<InvoiceDispatchException>(...)` buộc test runner chờ assertion hoàn tất.

## How to verify
Chạy `../verify.ps1`. Sau đó thử tạm thay production behavior để không còn phát sinh `InvoiceDispatchException`; regression test phải FAIL.

## Alternative fixes
Có thể trả trực tiếp task assertion từ test method nếu framework/version hỗ trợ signature phù hợp. `async Task` + `await` thường rõ ràng hơn khi test còn nhiều bước.

## Wrong / Tempting Fixes
- Thêm `Thread.Sleep`: không biến async assertion thành phần lifecycle của test.
- Gọi `.Wait()` hoặc `.Result`: có thể làm test đồng bộ hóa cưỡng bức nhưng làm giảm tính tự nhiên của async code và có thể che các vấn đề khác.
- Chỉ assert rằng method đã được gọi: không kiểm tra exception contract.

## Production implications
False-positive test làm CI tạo cảm giác an toàn giả cho failure path. Đây là rủi ro đặc biệt với retry, timeout, cancellation và exception handling.

## Trade-offs
Await trực tiếp làm test phản ánh đúng asynchronous contract, đổi lại test method cũng phải là async. Đây là trade-off rất nhỏ so với tính đúng đắn của regression coverage.

## Senior engineer should notice
Khi review async tests, kiểm tra không chỉ assertion có tồn tại mà còn xem completion của mọi `Task` quan trọng có được runner quan sát hay không.

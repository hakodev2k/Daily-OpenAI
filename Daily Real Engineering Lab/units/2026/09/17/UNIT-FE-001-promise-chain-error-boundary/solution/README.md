# Reference Solution — chỉ xem sau khi đã tự thử

## Symptoms
UI-level operation có thể ghi `SUCCESS` dù follow-up async work thất bại.

## Evidence
Event order của starter cho thấy outer chain hoàn tất độc lập với Promise của `followUp()`.

## Root cause
Callback đầu tiên của `.then()` khởi động `followUp()` nhưng không return Promise đó. Vì vậy parent chain nhận callback như đã hoàn tất ngay, chạy success continuation, còn failure được xử lý ở một nhánh Promise tách biệt.

## Why the fix works
Return Promise của `followUp()` để completion và rejection của nó trở thành một phần của chain đại diện cho toàn bộ user operation.

## How to verify
Chạy `./verify.ps1` trên code learner-editable trong `starter/`. Failure path phải đi tới error handling và không được ghi `SUCCESS`.

## Alternative fixes
Có thể viết cùng operation bằng `async/await`, miễn là mọi asynchronous step đều được `await` trong cùng operation boundary và error propagation không bị tách rời.

## Wrong / Tempting Fixes
- Thêm delay trước khi hiển thị success chỉ thay đổi timing, không sửa completion contract.
- Catch riêng follow-up rồi bỏ qua lỗi có thể hợp lệ nếu follow-up thật sự best-effort, nhưng khi business operation yêu cầu bước này thành công thì đó là thay đổi semantics.
- Disable button lâu hơn không giải quyết error propagation.

## Production implications
Sai completion boundary có thể tạo false-success UI, analytics sai, retry khó hiểu và state giữa client/server không nhất quán.

## Trade-offs
Một số side effect thực sự có thể là fire-and-forget. Khi đó cần định nghĩa rõ ownership, telemetry và failure policy thay vì vô tình tách Promise chain.

## Senior insight
Senior engineer không chỉ nhìn `then/catch`; cần xác định Promise nào đại diện cho business operation và bảo đảm lifecycle, cancellation/error handling bám theo boundary đó.
# Reference Solution — inspect only after reproducing and attempting your own fix

## Symptoms
Hai case pass riêng nhưng suite fail không ổn định khi overlap.

## Evidence
Timeline cho thấy một case có thể gọi reset sau khi case kia đã seed nhưng trước assertion của case kia.

## Root cause
Các test chia sẻ mutable fixture state và mỗi test tự reset toàn bộ state. `ConcurrentDictionary` làm từng operation thread-safe nhưng không tạo transaction/isolation cho lifecycle của một test.

## Why the fix works
Mỗi test cần sở hữu scope dữ liệu riêng: tạo fixture/store instance riêng cho mỗi case, hoặc namespace dữ liệu bằng test/run identifier và cleanup đúng scope. Khi ownership tách biệt, lifecycle của test này không phá setup của test khác.

## How to verify
Giữ parallel execution và chạy `verify.ps1` nhiều vòng.

## Alternative fixes
Nếu integration environment bắt buộc dùng chung database, dùng unique tenant/schema/database/key-prefix cho mỗi test run và cleanup theo owner. Serialize một nhóm test chỉ phù hợp khi resource thật sự không thể cô lập.

## Wrong / Tempting Fixes
- Tắt parallelization toàn suite: che coupling và làm CI chậm hơn.
- Thêm `lock` quanh từng dictionary operation: vẫn không bảo vệ toàn lifecycle setup → assertion.
- Tăng `Task.Delay`: chỉ thay xác suất race.

## Production implications
Test isolation kém tạo flaky CI, làm giảm niềm tin vào regression suite và có thể che concurrency assumptions trong production code.

## Trade-offs
Per-test fixture đơn giản nhưng có setup cost. Namespaced shared infrastructure nhanh hơn nhưng cleanup và quota phức tạp hơn.

## What a Senior engineer should notice
Thread safety và test isolation là hai thuộc tính khác nhau; cần xác định ownership boundary trước khi chọn synchronization.
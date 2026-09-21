# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Formatting check pass khi chạy riêng nhưng fail sau locale-specific check trong cùng process.

## 2. Evidence

`CultureInfo.CurrentCulture` bắt đầu là `en-US`, sau European scenario trở thành `de-DE`, và state đó vẫn còn khi check tiếp theo chạy.

## 3. Root cause

Test setup thay đổi ambient culture nhưng không restore giá trị trước đó. Test sau phụ thuộc ngầm vào default culture nên kết quả phụ thuộc execution order.

## 4. Why the fix works

Scope thay đổi culture trong một boundary có cleanup bảo đảm bằng `try/finally`, hoặc làm từng test thiết lập đầy đủ state mà nó cần. Điều này loại bỏ coupling qua ambient state.

## 5. How to verify

Chạy `verify.ps1`. Verification chạy European scenario trước rồi default scenario trong cùng process; cả hai phải pass.

## 6. Alternative fixes

Một test fixture có thể snapshot/restore culture cho mỗi test. Với code machine-readable, production API cũng có thể nhận explicit `CultureInfo` thay vì dựa vào ambient culture nếu contract phù hợp.

## 7. Wrong or misleading fixes

Đổi thứ tự test chỉ che coupling. Disable parallelization có thể cần cho một số global resources nhưng không sửa state leak tuần tự. Hard-code expected German output vào test mặc định sẽ làm sai contract.

## 8. Production implications

Ambient state như culture, timezone, environment variables, static caches và process-wide configuration dễ tạo flaky tests và behavior phụ thuộc host. Test isolation phải coi chúng là shared resources.

## 9. Trade-offs

Explicit dependency làm contract rõ hơn nhưng có thể tăng parameter plumbing. Scoped ambient state ít thay đổi production API hơn nhưng yêu cầu cleanup discipline.

## 10. What a Senior engineer should notice

Một test pass riêng không chứng minh isolation. Khi suite-only failure xuất hiện, cần tìm shared mutable state, execution ordering và cleanup guarantees trước khi đổ lỗi cho timing hoặc CI.
# Reference Solution — chỉ xem sau khi đã tự thử

## Symptoms

E2E test fail khi UI còn `Publishing...`, dù request hoàn tất sau đó.

## Evidence

Server cố định latency 650ms trong khi starter sleep 300ms rồi đọc DOM đúng một lần. Failure là deterministic trong lab nhưng mô phỏng CI flake do latency thực tế biến thiên.

## Root cause

Test đồng bộ bằng elapsed time thay vì observable application state.

## Why the fix works

Dùng Playwright web-first assertion:

```js
await page.getByRole('button', { name: 'Publish' }).click();
await expect(page.locator('#status')).toHaveText('Published');
```

Assertion retry cho đến khi condition đạt hoặc timeout, nên test mô tả điều kiện hoàn thành thay vì đoán duration.

## How to verify

Chạy `./verify.ps1`; script chạy chính file trong `starter/` mà learner sửa.

## Alternative fixes

Có thể chờ một response cụ thể nếu response đó chính là contract hoàn tất cần kiểm tra. Với UI behavior, web-first assertion thường diễn đạt outcome rõ hơn.

## Wrong / Tempting Fixes

- Tăng sleep từ 300ms lên 2s: giảm xác suất fail nhưng vẫn timing-dependent và làm suite chậm.
- Retry toàn test nhiều lần: che synchronization defect và tăng runtime.
- Giảm latency server: thay đổi hệ thống để chiều theo test, không sửa test contract.

## Production implications

E2E suite flaky làm giảm trust vào CI, tăng rerun và khiến regression thật dễ bị bỏ qua. Synchronization nên bám vào user-observable condition hoặc protocol event có ý nghĩa.

## Trade-offs

Timeout vẫn cần một upper bound để phát hiện hệ thống thực sự treo. Condition-based waiting không có nghĩa chờ vô hạn; nó tách readiness condition khỏi timing guess.

## Senior engineer nên nhận ra

Một E2E test tốt phải phân biệt ba thứ: action đã gửi, backend đã hoàn tất, và UI đã phản ánh outcome. Chọn wait condition dựa trên behavior contract cần chứng minh, không dựa trên tốc độ máy developer.

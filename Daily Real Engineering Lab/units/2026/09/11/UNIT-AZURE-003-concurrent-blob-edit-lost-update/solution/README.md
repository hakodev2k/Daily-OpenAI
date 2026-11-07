> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

# Reference Solution

## 1. Symptoms

Hai editor đọc cùng document version. Editor A save trước, editor B save sau. Starter cho phép cả hai write hoàn tất, nên edit B silently overwrites edit A.

## 2. Evidence

- A và B cùng đọc ETag `"v1"`.
- Save A thay state hiện tại sang version tiếp theo.
- Save B vẫn được chấp nhận dù snapshot của B đã stale.
- Không có network failure, retry hay storage outage.

## 3. Root cause

`DocumentService.SaveAsync` bỏ qua ETag đi kèm snapshot đã đọc và gọi write không có precondition. Vì write contract không ràng buộc với version mà editor đã chỉnh sửa, một stale editor vẫn có thể overwrite state mới hơn.

Trong Azure Blob Storage, ETag đại diện version của resource. Một update dựa trên state đã đọc nên dùng optimistic concurrency với conditional request như `If-Match`.

## 4. Why the fix works

Reference solution truyền `edited.ETag` vào write contract. Storage chỉ chấp nhận write nếu current ETag vẫn khớp ETag mà editor đã đọc.

Timeline sau fix:

1. A đọc `v1`.
2. B đọc `v1`.
3. A save với expected `v1` → success, blob trở thành `v2`.
4. B save với expected `v1` → precondition failure.
5. Application có thể yêu cầu B reload/merge thay vì silently overwrite A.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Expected:

- Save A completed.
- Save B bị reject vì stale version.
- Final content vẫn là `Edit from A`.
- Script in `VERIFY_PASS` và exit code `0`.

## 6. Alternative fixes

### Pessimistic/distributed lock

Có thể serialize edit bằng lock, nhưng tăng coordination, failure modes và operational complexity. Chỉ hợp lý khi business workflow thật sự cần exclusive editing.

### Merge workflow

Với rich text hoặc document collaboration, có thể detect conflict rồi merge. Đây là product-level strategy, không loại bỏ nhu cầu phát hiện stale write.

### Server-side version field riêng

Có thể dùng version number/application token thay ETag nếu persistence abstraction yêu cầu, miễn write vẫn compare-and-set atomically.

## 7. Wrong or misleading fixes

- **Retry stale write ngay lập tức:** chỉ lặp lại overwrite với version mới nếu retry không merge/reload business state.
- **Lock trong một process:** không bảo vệ khi application scale-out nhiều instance.
- **Last-write-wins và gọi đó là fix:** chỉ chấp nhận được khi business semantics cho phép mất update.
- **Đọc lại ngay trước write nhưng không có atomic condition:** vẫn còn race giữa read mới và write.

## 8. Production implications

Với Azure Blob Storage thực tế, map simulator concept sang Blob request condition (`If-Match`/ETag). Application cần translate precondition failure thành conflict semantics phù hợp, thường là HTTP `409 Conflict` hoặc `412 Precondition Failed` tùy boundary/API contract.

Telemetry nên ghi document id, expected version và conflict outcome nhưng không log sensitive content.

## 9. Trade-offs

Optimistic concurrency không ngăn conflict; nó biến silent data loss thành explicit conflict. Điều này giữ throughput và tránh global lock, nhưng application/UI phải có conflict handling path.

Nếu conflict cực kỳ thường xuyên, đó có thể là tín hiệu workflow hoặc data ownership cần thiết kế lại.

## 10. What a Senior engineer should notice

- Concurrency correctness nằm ở atomic write condition, không nằm ở preflight check.
- Read metadata chỉ có giá trị nếu được carry qua write boundary.
- Storage primitive phải được map thành business conflict semantics rõ ràng.
- Retry policy không được tự động retry optimistic-concurrency conflict như transient failure.
- Last-write-wins có thể là policy hợp lệ, nhưng phải là quyết định business có chủ ý chứ không phải behavior mặc định vô tình.

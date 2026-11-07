# Reference Solution — CHỈ ĐỌC SAU KHI TỰ THỬ

## Symptoms
Unit test dùng fake xanh và command trả `true`, nhưng một lượt đọc độc lập sau request vẫn thấy `Pending`. Điều này cho thấy cần phân biệt kết quả của service với trạng thái đã được lưu.

## Evidence
`IdentityMapFakeStore.FindAsync` trả chính instance đang được giữ trong dictionary. `DetachedPermitStore.FindAsync` trả snapshot độc lập. Cùng một thao tác mutation trong service tạo hai kết quả khác nhau sau `ReloadAsync`. Trước fix, `SaveCount=0`.

## Root cause
Business service thay đổi mutable entity trong bộ nhớ rồi trả thành công mà không gọi persistence operation. Test fake dùng shared reference nên tự động phản ánh mutation, che giấu việc thiếu lưu. Đây là **test-double contract mismatch**, không phải database timeout.

## Fix và vì sao hiệu quả
Tham khảo [PermitApprovalService.cs](PermitApprovalService.cs). Sau khi transition hợp lệ, service gọi `await _store.SaveAsync(permit)` trước khi trả `true`. `SaveAsync` tạo snapshot lưu mới, nên trạng thái tồn tại sau `ReloadAsync` ở cả hai implementation. Trong project của bạn, sửa **`starter/PermitApprovalService.cs`**; không thay bằng cách chạy reference code.

Bổ sung test theo detached persistence semantics (hoặc thay fake hiện tại bằng fake snapshot-aware) để assertion luôn thực hiện một lần reload mới và kiểm tra số lần ghi. Không chỉ assert `ApproveAsync` trả `true`.

## Verify
Từ root của unit chạy `./scripts/verify.ps1`. Script chỉ build và chạy `starter/PermitLab.csproj`, kiểm tra trạng thái, version, đúng một lần lưu, request lặp và permit không tồn tại. Không có test nào cần database thật.

## Alternative fixes
- Fake snapshot-aware: phù hợp unit test nhanh, nhưng vẫn cần contract/integration test với persistence adapter thật.
- Integration test với database local: tăng độ tin cậy về transaction/ORM behavior, đổi lại tốn setup và chạy chậm hơn.
- Unit of Work/EF Core tracking: hợp lệ khi contract quy định rõ `SaveChangesAsync` và lifetime của DbContext; không nên dựa vào side effect ngầm.

## Wrong / tempting fixes
- Chỉ assert `accepted=true`: không chứng minh trạng thái đã bền vững.
- Sửa `DetachedPermitStore` để trả cùng instance: làm test xanh bằng cách làm yếu mô hình production.
- Retry command vô hạn: vẫn không bổ sung persistence boundary.
- Đổi `Status` thành static/shared: che lỗi trong process và phá isolation giữa hồ sơ.

## Production implications và trade-offs
Trong hệ thống thực, Save phải gắn với transaction boundary, error propagation, concurrency token và idempotency. `SaveAsync` có thể throw; service không được trả thành công khi ghi thất bại. Với approval đồng thời, optimistic concurrency (`rowversion`/ETag) hoặc transaction kiểm tra điều kiện cần được cân nhắc. Fake không thay thế được test với adapter thật.

## Senior engineer cần nhận ra
Test doubles phải giữ **observable contract** của dependency, không chỉ cùng interface. Khi nghiệp vụ nói “đã duyệt”, cần xác định chính xác khi nào dữ liệu được commit, trạng thái nào quan sát được từ request khác, và test nào chứng minh điều đó.

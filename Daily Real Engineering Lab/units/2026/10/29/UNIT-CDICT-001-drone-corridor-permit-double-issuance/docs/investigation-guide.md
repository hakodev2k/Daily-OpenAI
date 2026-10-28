# Investigation guide — trước khi xem solution

1. Chạy baseline, ghi số external issuance, permit được trả cho hai caller và số lần cấp khi gọi lại cùng khóa.
2. Phân biệt ba giả thuyết: hai khóa không giống nhau, fake authority tự retry, hoặc một thao tác cache không đảm bảo side effect chỉ chạy một lần.
3. Đặt breakpoint trong `PermitRegistry.GetOrIssue` và `FakePermitAuthority.Issue`. Quan sát thứ tự hai worker.
4. Xác định contract thực sự cần: cache chỉ có một value hay external issuance cũng phải đúng một lần?
5. Sau khi sửa, chạy `verify.ps1`. Đề xuất cách xử lý exception từ external authority và giới hạn đảm bảo trong nhiều process/pod.

Ghi hypothesis, evidence và trade-off trong [workspace](../workspace/my-investigation.md).

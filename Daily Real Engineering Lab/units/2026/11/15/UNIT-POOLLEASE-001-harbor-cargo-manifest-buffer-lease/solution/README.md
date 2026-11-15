# Spoiler — Reference Solution

`ReadOnlyMemory<byte>` chỉ là view, không có ownership. `ManifestService.Read` trả view trỏ vào rented array rồi `Return` array trước khi caller dùng xong. Lần ingest kế tiếp reuse và ghi đè cùng array, làm hỏng manifest trước mà không cần concurrency.

Reference trả `bytes.AsMemory()` từ array riêng. Production nên bỏ hẳn rent/return nếu chọn owned copy; một lựa chọn khác là lease object (`IMemoryOwner<byte>`) với lifecycle rõ ràng. Đổi lại copy/allocate tăng chi phí, nên đo benchmark trước khi tối ưu.

Wrong fixes: `lock` không cứu dữ liệu sau return; `ReadOnlyMemory` không copy; `Return(clearArray:true)` làm view mất dữ liệu sớm hơn; tăng pool size không bảo đảm isolation.

Chạy `verify.ps1` trên starter sau khi tự sửa. Thử thêm 3 lượt ingest, length khác nhau và concurrent readers. Validation hiện tại là static-review, không tuyên bố đã thực thi .NET.

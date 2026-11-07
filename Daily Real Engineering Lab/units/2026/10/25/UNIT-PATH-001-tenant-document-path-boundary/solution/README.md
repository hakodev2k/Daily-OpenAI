# Reference Solution — chỉ đọc sau khi tự investigate

## 1. Symptoms
Một request có tenant identity hợp lệ đọc được file trong thư mục backup nằm **cạnh** thư mục tenant. Request khác đi ra một thư mục sibling không trùng tiền tố lại bị từ chối. Tài liệu hợp lệ vẫn đọc bình thường.

## 2. Evidence
Harness cho thấy `similar-sibling` trả `ALLOWED; content=restricted`, còn `unrelated-sibling` trả `DENIED`. Log không cho thấy authentication thất bại hoặc lỗi backend storage. Bằng chứng tập trung vào cách xác định ranh giới tài nguyên, không phải JWT hay CDN.

## 3. Root cause
`Path.GetFullPath` chuẩn hóa `..`, nhưng `StartsWith(root)` là phép so sánh **chuỗi**, không phải kiểm tra quan hệ thư mục. Với root `.../tenant-42`, sibling `.../tenant-42-backup` vẫn có cùng prefix, nên một file ngoài tenant bị coi là hợp lệ. Dùng `OrdinalIgnoreCase` trên Linux còn có thể tạo sai khác so với quy tắc phân biệt hoa/thường của filesystem.

## 4. Why the fix works
Xem [PathPolicy.cs](PathPolicy.cs). Yêu cầu path tương đối, tính canonical root/candidate, rồi dùng `Path.GetRelativePath(root, candidate)`. Chỉ chấp nhận đường dẫn tương đối nằm dưới root; từ chối `..`, tiền tố `../` (theo separator của platform), đường dẫn rooted và chính thư mục root. Việc kiểm tra theo segment tránh nhầm sibling trùng tiền tố. API của `Path` sử dụng semantics của platform.

## 5. How to verify
Sao chép nội dung `solution/PathPolicy.cs` vào `starter/PathPolicy.cs` **sau khi đã thử tự sửa**. Chạy `./verify.ps1`: valid vẫn `ALLOWED`, hai sibling đều `DENIED`, exit 0. `./reproduce.ps1` vẫn exit 0 để chứng minh baseline ban đầu. Không thay đổi `harness/` để làm xanh test.

## 6. Alternative fixes
- Kiểm tra prefix đã có separator cuối root và dùng so sánh case phù hợp platform: có thể giải quyết case này nhưng dễ sai với root, path canonicalization và platform khác nhau.
- Không nhận path từ client: ánh xạ document ID sang storage key do server quản lý và kiểm tra tenant ownership trong database; đây thường là thiết kế production tốt hơn.
- Dùng object storage với authorization theo tenant/object key và identity scoped, nếu đã có hạ tầng phù hợp.

## 7. Wrong / tempting fixes
- Chỉ xóa chuỗi `../`: không phải boundary check đáng tin; các biểu diễn đường dẫn khác và normalization làm giả định không bền.
- Chỉ `GetFullPath`: chuẩn hóa không tự động áp quyền truy cập.
- Chỉ so sánh `StartsWith(root)`: bỏ sót segment boundary.
- Chỉ tin JWT hợp lệ: authentication không đồng nghĩa với quyền đọc từng tài liệu.
- Chỉ chặn tên `backup`: vá một tên thư mục, không sửa policy.

## 8. Production implications
Đây là **lab lexical path containment**, không phải sandbox filesystem hoàn chỉnh. Symbolic link/reparse point nằm bên trong root có thể trỏ ra ngoài; đường dẫn có thể thay đổi giữa kiểm tra và mở file (TOCTOU). Đối với production, ưu tiên document ID + tenant ownership check, file permissions tách biệt, storage root không thể ghi tùy ý, kiểm soát link và mở file theo handle/OS-enforced boundary khi threat model yêu cầu. Không log nội dung tài liệu hay token. Kiểm tra permission, normalize đường dẫn và authorization tại thời điểm truy cập.

## 9. Trade-offs
Canonical relative-path validation nhỏ, nhanh và phù hợp khi cây thư mục được server quản lý, không có symlink không tin cậy. Document-ID mapping tăng truy vấn metadata nhưng tách API contract khỏi layout storage. OS isolation/storage IAM tăng defense-in-depth nhưng cần vận hành và kiểm thử quyền. Cần đánh giá theo threat model, không mặc định một helper string là đủ.

## 10. What a Senior engineer should notice
Tách identity, resource ownership, path canonicalization, filesystem authority và observability. Viết regression cho **positive case** và **negative case**; kiểm tra Windows/Linux, Unicode/case semantics, link, race, rollback. Khi rollout, audit các access bị từ chối theo tenant và path category đã được redacted, không log path nhạy cảm đầy đủ.

## Tài liệu chính thức
- .NET `Path.GetFullPath`: https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath
- .NET `Path.GetRelativePath`: https://learn.microsoft.com/dotnet/api/system.io.path.getrelativepath
- .NET `Path.IsPathFullyQualified`: https://learn.microsoft.com/dotnet/api/system.io.path.ispathfullyqualified

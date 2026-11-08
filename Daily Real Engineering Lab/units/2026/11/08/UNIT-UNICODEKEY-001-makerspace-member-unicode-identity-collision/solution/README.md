# Reference Solution — spoiler

## Root cause
Chuỗi `Café-72` có thể biểu diễn bằng ký tự `é` dựng sẵn (U+00E9) hoặc `e` + combining acute accent (U+0065 U+0301). `StringComparer.Ordinal` coi chúng là khác nhau. Vì key được tạo mà không chuẩn hóa canonical Unicode, HashSet chấp nhận cả hai.

## Fix
Thay `IdentityKey` bằng `submitted.Trim().Normalize(NormalizationForm.FormC)`. Xem `solution/Program.cs`; sửa **starter** rồi chạy `./scripts/verify.ps1`. Form C chỉ xử lý canonical equivalence, không tự quyết định case-insensitive hay compatibility equivalence.

## Wrong fixes
- Dùng `ToLower()` theo current culture: không giải quyết canonical equivalence, tạo thêm rủi ro locale.
- So sánh bằng mắt hoặc dùng length: không phải identity contract.
- Xóa record trùng thủ công: lỗi tiếp tục tái phát.
- Áp dụng NFKC bừa bãi: có thể gộp những identifier mà nghiệp vụ muốn phân biệt.

## Production trade-offs
Thực tế phải enforce canonical unique key tại DB với unique constraint và race-safe insert; migrate/backfill sau khi audit collisions hiện có. Định nghĩa policy rõ ràng về trimming, case folding, Unicode version và các ký tự được phép. Phân biệt display name với immutable account identifier.

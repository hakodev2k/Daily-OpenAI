# Sau khi sửa
- `./scripts/verify.ps1` exit 0 và in `PASS: cache hit, cache miss, 24 concurrent readers, exactly-once creation.`
- `STABLE` vẫn là 8; `NEW` và `ANOTHER` là 12; `CreatedCount` lần lượt là 1 và 2.
- `./scripts/reproduce.ps1` không còn PASS là bình thường: lỗi gốc đã biến mất.
- Không đánh giá chỉ dựa vào tốc độ máy; ưu tiên invariant về correctness và số lần tạo.

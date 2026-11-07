# Trước khi sửa
- reproduce.ps1: exit 0 và in REPRODUCED.
- Dữ liệu A đúng ngay sau Prepare(A), nhưng sau Prepare(B) thì A có thể mang nội dung B trong fixture tái sử dụng deterministic.
- verify.ps1: exit khác 0, ít nhất kiểm tra first survives second request và first remains stable after more requests thất bại.
- Không suy luận rằng ArrayPool.Shared luôn tái sử dụng cùng buffer: fixture cố ý kiểm soát lịch reuse để chứng minh một hành vi hợp lệ.

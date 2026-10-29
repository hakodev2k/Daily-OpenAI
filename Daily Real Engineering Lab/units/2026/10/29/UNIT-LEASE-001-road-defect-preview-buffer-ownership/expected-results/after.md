# Sau khi sửa
- verify.ps1: exit 0, tất cả kiểm tra PASS, bao gồm A ổn định qua request tiếp theo, không phụ thuộc thay đổi mảng nguồn, hỗ trợ empty và large payload.
- reproduce.ps1 trên starter đã sửa: exit khác 0 và NOT REPRODUCED; đây là kỳ vọng vì script này chỉ dùng để chứng minh bug gốc.
- Không cần benchmark latency cố định. Đánh giá thêm allocation/copy cost với kích thước ảnh thật trước khi rollout.

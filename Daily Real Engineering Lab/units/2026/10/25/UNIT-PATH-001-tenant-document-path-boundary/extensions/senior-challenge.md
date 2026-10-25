# Senior extension — harden the boundary

1. Thiết kế API nhận `documentId` thay vì client-provided path. Mô tả schema tenant ownership, authorization check và cách audit access denied.
2. Viết threat model cho symbolic link/reparse point, TOCTOU giữa validation và file open, và quyền ghi thư mục tenant.
3. Đề xuất bộ integration tests trên Windows và Linux. Giải thích case sensitivity, separator, relative/absolute path, symlink và path thay đổi trong lúc đọc.
4. Đánh giá phương án local filesystem vs object storage (IAM, cost, vận hành, latency) với 500 tenants và 2 triệu tài liệu.

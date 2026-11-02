# Senior extension — Đổi chính sách đối tác mà không sửa lịch sử

Giả sử đối tác A dùng `AwayFromZero`, đối tác B dùng `ToEven`; các giao dịch trước 2027-01-01 phải giữ policy cũ. Thiết kế dữ liệu lưu `policyVersion` cùng bút toán, contract tests cho replay và cách so sánh báo cáo cũ/mới. Không cần triển khai trong lab nền tảng. Đánh giá backward compatibility, auditability và rollback.

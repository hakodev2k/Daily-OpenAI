# Decision brief — Municipal housing repair portal

## Requirements
- Hai replica cùng xử lý session ticket được phát hành bởi replica bất kỳ.
- Rolling restart không được khiến ticket đang hợp lệ tự nhiên mất hiệu lực.
- Ứng dụng quản trị khác phải không giải mã được ticket của portal, kể cả khi dùng chung hạ tầng lưu key.
- Không lưu secret/key trong source control; không in protected ticket vào log.
- Không phụ thuộc sticky sessions để đạt tính đúng đắn.
- Duy trì khả năng khôi phục sau sự cố, có key rotation và phân quyền đọc/ghi rõ ràng.

## Constraints
- 2 replica hiện tại, mở rộng lên 6 trong 12 tháng.
- Thời hạn ticket tối đa 8 giờ; audit tối thiểu 30 ngày.
- Chỉ có một nhóm backend nhỏ, ưu tiên ít thành phần phải vận hành.
- Lab dùng local directory để minh họa hợp đồng; production phải chọn storage bền vững và cơ chế bảo vệ key at rest phù hợp.

## Deliverable
Viết ADR tối đa một trang: decision, assumptions, alternatives, threat model, rollout plan, verification signals và rollback. Giải thích sự khác nhau giữa shared key ring, session affinity, server-side session và việc dùng chung ApplicationName. Không thay đổi mục tiêu cách ly ứng dụng chỉ để làm test xanh.

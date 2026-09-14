# Scenario

Hệ thống B2B order management hiện lưu trạng thái hiện tại trong SQL Server. Business yêu cầu bổ sung audit history có thể truy vấn trong 7 năm.

## Requirements

- Truy vấn được ai thay đổi, thời điểm, field thay đổi, giá trị trước/sau.
- Không được làm write path chính trở nên quá phức tạp.
- Audit data tăng nhanh hơn current-state data nhiều lần.
- Retention 7 năm, sau đó phải purge theo policy.
- Một số truy vấn audit chỉ phục vụ support/compliance, không phải request path chính.
- Team 4 backend developers, không có data platform team riêng.
- Hệ thống hiện dùng SQL Server và Azure.
- Schema business có thể thay đổi vài lần mỗi năm.

## Options tối thiểu cần đánh giá

1. SQL Server temporal tables.
2. Audit/history tables riêng trong cùng database.
3. Append-only audit store tách khỏi transactional model.

Bạn có thể đề xuất hybrid nếu có lý do rõ ràng.

## Deliverable

Trong `workspace/my-decision.md`, ghi:

- assumptions
- decision criteria
- comparison table
- selected design
- write-path implications
- read/query implications
- retention/purge strategy
- schema-evolution implications
- điều kiện để revisit quyết định

# Reference Solution — chỉ đọc sau khi tự reproduce và sửa

## Symptoms

Đồng bộ metadata thành công, nhãn Equipment P-017 đổi, nhưng hai Inspections biến mất. Không có exception; PRAGMA foreign_key_check vẫn trả 0.

## Evidence

Fixture tạo P-017 và I-001/I-002. Sync lần đầu làm child count từ 2 thành 0; sync lặp lại không khôi phục chúng. P-099 mới vẫn được thêm.

## Root cause

SQLite INSERT OR REPLACE không phải UPDATE-in-place. Khi xung đột PRIMARY KEY, SQLite xóa hàng Equipment cũ rồi chèn hàng mới. Inspections có foreign key ON DELETE CASCADE nên các hàng con bị xóa hợp lệ.

## Why the fix works

Thay bằng INSERT ... ON CONFLICT(Id) DO UPDATE SET Label = excluded.Label. Hàng cũ được cập nhật, không bị xóa; các Inspections còn nguyên. Thiết bị mới vẫn được INSERT. Xem [reference code](EquipmentImporter.cs).

## How to verify

Tự sửa starter/EquipmentImporter.cs rồi chạy ./verify.ps1. Script dùng chính learner-editable starter, kiểm tra child count sau cả hai lần sync, nhãn mới, thiết bị mới và FK check. Sau khi tự làm, có thể copy solution/EquipmentImporter.cs sang starter/EquipmentImporter.cs để so sánh.

## Alternative fixes

SELECT rồi UPDATE/INSERT có thể đúng nhưng phải xét transaction và concurrent writers. UPDATE trước rồi INSERT nếu không có hàng cũng cần race handling. UPSERT có version predicate phù hợp nếu yêu cầu optimistic concurrency.

## Wrong / tempting fixes

Tắt foreign keys tạo orphan records. Bỏ ON DELETE CASCADE có thể biến mất dữ liệu thành exception nhưng không sửa write semantics. Backup/restore child rows quanh mỗi sync thêm race và overhead. Chỉ kiểm tra foreign_key_check bỏ sót cascade deletion hợp lệ nhưng sai nghiệp vụ.

## Production implications

Audit các bản ghi đã mất, đối chiếu backup/event logs để xác định khả năng phục hồi; không giả định có thể khôi phục nếu không có nguồn dữ liệu. Dùng transaction cho batch sync, thêm metric child-count và regression tests trên SQLite thật.

## Trade-offs

UPSERT một statement đơn giản và atomic cho metadata sync. Khi có stale offline writers, cần versioning và chính sách conflict rõ ràng; multi-table operations cần transaction phù hợp.

## What a Senior engineer should notice

Successful SQL execution không đồng nghĩa bảo toàn business invariants. Kiểm thử dữ liệu không được thay đổi, hiểu dialect-specific behavior, cascade effects, idempotency và khả năng rollback.

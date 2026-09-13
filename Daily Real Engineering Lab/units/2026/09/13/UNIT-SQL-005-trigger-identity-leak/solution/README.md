# Reference Solution — chỉ xem sau khi đã tự thử

## Symptoms

Business row được tạo thành công nhưng ID trả về cho caller có thể trỏ sang row được tạo bởi trigger.

## Evidence

`dbo.Receipts` và `dbo.AuditLog` cùng sinh identity value. Trigger chạy trong cùng session sau business insert.

## Root cause

`@@IDENTITY` trả identity value cuối cùng được sinh trong session, không giới hạn ở statement hoặc scope mà caller quan tâm. Audit trigger tạo thêm một identity, nên business code đọc nhầm identity của audit row.

## Why the fix works

Reference solution dùng `OUTPUT inserted.Id` để capture trực tiếp identity của row do chính `INSERT INTO dbo.Receipts` tạo ra. Contract trả ID được gắn với statement tạo business entity, không phụ thuộc side effect trong trigger.

## Verification

Copy logic từ `solution/create-receipt.sql` sang `starter/create-receipt.sql`, sau đó chạy:

```powershell
./scripts/verify.ps1
```

## Alternative fixes

`SCOPE_IDENTITY()` cũng tránh identity được sinh ở nested trigger scope trong trường hợp single-row insert. `OUTPUT inserted.Id` rõ ràng hơn khi cần capture chính các row do statement tạo và mở rộng tự nhiên sang multi-row insert.

## Wrong / Tempting Fixes

- Disable trigger: làm mất audit requirement và né root cause.
- Dùng `IDENT_CURRENT('Receipts')`: giá trị là table-wide, không an toàn khi có concurrent inserts từ session khác.
- Lấy `MAX(Id)`: race condition dưới concurrency.
- Giữ `@@IDENTITY` rồi kiểm tra row sau đó: vẫn để contract sai và chỉ phát hiện hậu quả muộn.

## Production implications

Identity retrieval là một phần của data-access contract. Trigger, replication hoặc thay đổi schema có thể làm lộ assumption scope sai mà trước đó không xuất hiện trong môi trường đơn giản.

## Trade-offs

`OUTPUT inserted.Id` explicit và phù hợp batch insert, nhưng data-access layer phải đọc result set. `SCOPE_IDENTITY()` đơn giản cho single-row insert nhưng vẫn cần hiểu rõ scope semantics.

## What a Senior engineer should notice

Không chỉ sửa một function call. Hãy xác định ownership boundary của generated identifiers, kiểm tra concurrency, trigger side effects và regression test cho contract “ID trả về chính là entity vừa tạo”.

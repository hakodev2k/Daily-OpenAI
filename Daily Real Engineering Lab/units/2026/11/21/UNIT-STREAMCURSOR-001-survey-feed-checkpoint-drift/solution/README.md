# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Batch đầu trả một bản ghi; batch sau rỗng dù feed còn ba dòng.

## Evidence
Dòng đầu đã đọc nhưng checkpoint có thể bằng độ dài toàn file; so sánh `BaseStream.Position` và offset byte ngay sau newline đầu tiên.

## Root cause
`StreamReader` đọc trước nhiều byte vào buffer. `BaseStream.Position` phản ánh vị trí của stream nền, không phải vị trí byte logic của dòng vừa trả về. Lưu vị trí này làm checkpoint nhảy qua các dòng chưa xử lý.

## Why the fix works
Reference `Program.cs` đọc byte đến newline trên `FileStream`, cập nhật offset đúng sau mỗi record. Dùng UTF-8 strict decode; xử lý CRLF và dòng cuối không có newline.

## How to verify
Chép logic `CheckpointReader.ReadBatch` từ solution vào starter, chạy `./verify.ps1`. Kỳ vọng bốn ID đúng thứ tự, không thiếu/nhân đôi.

## Alternative fixes
Lưu số record đã commit rồi đọc bỏ qua số record đó khi resume: đơn giản nhưng O(n) cho lần restart; lưu offset theo byte sau delimiter là O(1) seek. Nếu cần durability, commit checkpoint atomically cùng dữ liệu đã nhập hoặc dùng idempotent upsert.

## Wrong / tempting fixes
Tăng buffer size hoặc gọi `DiscardBufferedData` sau khi đã lưu offset không khôi phục ranh giới logic; dùng `StreamReader.Peek` cũng có thể đọc trước. Chỉ ghi checkpoint sau khi import thành công, không chỉ sau khi đọc.

## Production implications
Crash giữa write và checkpoint có thể gây duplicate; cần idempotency theo record ID. Nếu file thay đổi giữa các lần chạy, phải pin file version/ETag, checksum hoặc generation ID.

## Trade-offs
Đọc byte từng byte minh họa tính đúng nhưng kém hiệu quả cho file lớn; production nên dùng buffered byte parser giữ byte-offset chính xác. Xử lý BOM, encoding, newline và maximum line length theo contract.

## What a Senior engineer should notice
Checkpoint là một protocol giữa parser, durable sink và source version, không chỉ một số nguyên. Verify cả crash-window, replay và consistency, không chỉ happy path.

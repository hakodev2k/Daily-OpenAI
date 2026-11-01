# Reference Solution — chỉ xem sau khi tự verify

## 1. Symptoms

Dashboard đôi khi nhận file rỗng; exporter thất bại làm mất snapshot cũ.

## 2. Evidence

DURING bytes=0, AFTER revision18=True, INTERRUPTED oldRevisionPreserved=False. Hai lỗi độc lập: visibility và rollback.

## 3. Root cause

FileMode.Create truncate file public **ngay khi mở**, trước khi payload mới hoàn chỉnh. Reader có thể đọc trạng thái dở dang; exception để lại file rỗng.

## 4. Why the fix works

Ghi toàn bộ JSON vào file tạm cùng thư mục, flush và đóng, rồi mới thay thế đường dẫn public bằng File.Move(..., overwrite: true). Nếu exception xảy ra trước thay thế, file cũ vẫn còn; finally dọn file tạm.

## 5. How to verify

Sau khi tự sửa, có thể copy solution/SnapshotPublisher.cs sang starter/SnapshotPublisher.cs rồi chạy ./verify.ps1. Harness kiểm tra reader, commit, exception và cleanup trên **starter**.

## 6. Alternative fixes

File.Replace với backup (phải kiểm tra platform), immutable versioned objects + pointer, hoặc transaction DB nếu cần nhất quán nhiều bản ghi.

## 7. Wrong / tempting fixes

Thread.Sleep chỉ giảm xác suất race; lock riêng writer không ràng buộc reader độc lập; Flush(true) sau truncate không phục hồi bản cũ; catch-and-ignore che dấu hỏng dữ liệu.

## 8. Production implications

Đặt temp cùng volume, dùng tên duy nhất, giám sát lỗi rename, cleanup file tạm sau crash, single-writer ownership, checksum/version và cảnh báo snapshot quá cũ.

## 9. Trade-offs

Local filesystem thường hỗ trợ atomic visibility khi rename/replace cùng volume; network share và object store có semantics khác. File.Flush(true) không tự bảo đảm fsync metadata thư mục trên mọi OS. Power-loss durability cần thiết kế và kiểm thử riêng. Hai writer cạnh tranh cần thêm version/lock.

## 10. Senior engineer should notice

Định nghĩa invariant trước khi sửa; test cả failure path, không chỉ happy path. Đánh giá đúng ranh giới filesystem, multi-writer và crash consistency.

[Reference source](SnapshotPublisher.cs)

# Reference Solution — chỉ xem sau khi đã tự điều tra

## 1. Symptoms

Starter cho phép đọc file trong `uploads`, nhưng cũng chấp nhận file trong một sibling directory như `uploads-archive` nếu canonical path của sibling bắt đầu bằng cùng chuỗi ký tự.

## 2. Evidence

Ví dụ lexical paths:

```text
allowed:   .../uploads
valid:     .../uploads/invoice.txt
outside:   .../uploads-archive/payroll.txt
```

Cả `valid` và `outside` đều có thể thỏa điều kiện prefix với chuỗi `.../uploads`, dù chỉ path đầu tiên là descendant của directory được phép.

## 3. Root cause

`StartsWith(_allowedRoot)` kiểm tra **string prefix**, không chứng minh **directory containment**. Boundary giữa directory name và path segment tiếp theo không được đưa vào invariant.

## 4. Why the fix works

Reference solution canonicalize allowed root, chuẩn hóa trailing separator và kiểm tra target bằng prefix có directory separator ở cuối root. Khi đó:

```text
.../uploads/invoice.txt
```

match boundary:

```text
.../uploads/
```

nhưng:

```text
.../uploads-archive/payroll.txt
```

không match.

## 5. How to verify

Sau khi áp dụng fix vào `starter/`:

```powershell
./verify.ps1
```

Verification yêu cầu:

- valid file vẫn đọc được
- outside file bị reject
- confidential content không xuất hiện trong output
- process exit code bằng 0

## 6. Alternative fixes

Một abstraction an toàn hơn có thể không nhận arbitrary absolute path từ caller. Ví dụ caller chỉ truyền logical document ID hoặc relative file name; storage layer tự resolve từ một trusted root. Cách này giảm surface area thay vì cố validate mọi path do caller cung cấp.

Trên hệ thống cần hỗ trợ case-insensitive filesystem contract, comparer cũng phải phù hợp với platform/volume semantics thay vì mặc định cho rằng mọi filesystem giống nhau.

## 7. Wrong / Tempting Fixes

### Chặn chuỗi `..`

Không giải quyết bản chất boundary và dễ bỏ sót alternate representations. Sau canonicalization, vấn đề trong lab thậm chí không cần `..`.

### Chặn riêng `uploads-archive`

Đây là deny-list cho một sample cụ thể, không thiết lập invariant tổng quát.

### Chỉ gọi `Path.GetFullPath`

Canonicalization cần thiết nhưng không tự chứng minh target thuộc allowed directory.

### Bắt exception từ `File.ReadAllText`

I/O exception không phải authorization policy.

## 8. Production implications

Filesystem authorization nên được thiết kế theo allow-list boundary rõ ràng và caller nên có ít quyền biểu diễn path nhất có thể. Nếu application chạy trên storage có symlink/reparse point hoặc mount behavior phức tạp, lexical canonical-path validation có thể vẫn chưa đủ; khi đó cần policy xử lý link/real target phù hợp với OS và threat model.

## 9. Trade-offs

Prefix-with-separator là fix nhỏ, dễ hiểu cho lexical path containment trong phạm vi lab. Thiết kế document-ID-to-storage-path mạnh hơn về boundary nhưng yêu cầu thay đổi API/storage abstraction. Real-path/symlink-safe enforcement phức tạp hơn và chỉ nên thêm khi threat model yêu cầu.

## 10. Senior engineer should notice

Senior engineer không chỉ hỏi “chuỗi này có bắt đầu giống root không?” mà phải định nghĩa security invariant: **resource cuối cùng được mở phải thuộc storage boundary mà principal được phép truy cập**. Validation phải chứng minh invariant đó, và threat model phải nói rõ có cần xử lý symlink/reparse point hay không.

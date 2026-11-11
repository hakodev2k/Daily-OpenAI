# Reference Solution

## Root cause
`JsonElement` phụ thuộc vào `JsonDocument` sở hữu backing storage. Trả view ra ngoài `using` khiến truy cập ở bước đối soát ném `ObjectDisposedException`.

## Fix
`return payload.Clone();` tạo snapshot có backing storage riêng trước khi document bị dispose. Xem `solution/Program.cs`; sửa tương ứng trong `starter/Program.cs` rồi chạy `./scripts/verify.ps1`.

## Wrong fixes
- Bắt exception rồi bỏ bản ghi: làm mất giấy phép.
- Bỏ dispose mà không quản lý lifetime: tạo ownership không rõ ràng.
- Parse lại mỗi bước: lãng phí và không giải quyết hợp đồng sở hữu.

## Trade-offs
Clone cấp phát bộ nhớ theo subtree. Với payload lớn, DTO typed hoặc document ownership xuyên pipeline có thể phù hợp hơn. Regression so sánh cả hai ID và quota; không chỉ kiểm tra absence of exception.

## Validation
Static review only; .NET và PowerShell chưa được thực thi trong môi trường xuất bản.

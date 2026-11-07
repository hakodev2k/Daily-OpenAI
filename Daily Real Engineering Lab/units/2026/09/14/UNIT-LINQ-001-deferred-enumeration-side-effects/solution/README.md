# Reference Solution — chỉ xem sau khi đã tự điều tra

## 1. Symptoms
Kết quả invoice đúng nhưng 4 SKU tạo ra 8 catalog lookups.

## 2. Evidence
`lines` được dùng bởi cả `Sum` và `Count`; mỗi operation duyệt sequence một lần.

## 3. Root cause
`Select` tạo deferred `IEnumerable`. Projection chứa dependency call có side effect. Mỗi lần enumerate, projection chạy lại và gọi catalog lại.

## 4. Why the fix works
Materialize kết quả sau khi pricing hoàn tất tạo một evaluation boundary. Các bước thống kê sau đó đọc dữ liệu đã sở hữu thay vì thực thi lại projection.

## 5. How to verify
Chạy `verify.ps1`; total phải là 100, expensive count là 2 và catalog calls là 4.

## 6. Alternative fixes
Có thể thiết kế pipeline chỉ enumerate một lần nếu không cần reuse. Với remote catalog thật, batch API hoặc explicit pricing stage cũng có thể phù hợp hơn.

## 7. Wrong or misleading fixes
Tăng rate limit hoặc thêm retry chỉ che load amplification. Cache toàn cục có thể giảm call nhưng thêm consistency semantics không cần thiết cho root cause này.

## 8. Production implications
Side effect bên trong deferred pipeline có thể biến một operation tưởng như local thành nhiều network/database calls, tăng latency, cost và nguy cơ rate limiting.

## 9. Trade-offs
Materialization dùng thêm memory và thay đổi streaming behavior. Nó phù hợp khi tập dữ liệu hữu hạn, cần reuse và side effect phải xảy ra đúng một lần cho mỗi item.

## 10. What a Senior engineer should notice
`IEnumerable` không chỉ là collection; nó có thể là một computation. Trước khi enumerate nhiều lần, cần biết evaluation semantics, ownership và cost của sequence.

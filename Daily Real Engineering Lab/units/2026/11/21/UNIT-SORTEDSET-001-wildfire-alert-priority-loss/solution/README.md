# Reference Solution — Spoiler

## Symptoms
5 ID đầu vào, chỉ 3 ID trong output.

## Evidence
Các alert Severity=5 cạnh tranh một vị trí; `SortedSet.Add` không báo exception.

## Root cause
`SortedSet<T>` sử dụng `IComparer<T>` cho cả thứ tự lẫn tính tương đương. Comparer chỉ xét Severity nên trả 0 với hai ID khác nhau có cùng Severity, làm rơi sự kiện.

## Why the fix works
`solution/Program.cs` so sánh Severity giảm dần, sau đó ID ordinal tăng dần. Hai alert khác ID không còn bị xem là cùng phần tử.

## How to verify
Copy `solution/Program.cs` đè `starter/Program.cs`, chạy `./verify.ps1`. Kiểm tra cả input đảo ngược và ID trùng.

## Alternative fixes
Batch sort `List<Alert>` và explicit dedup theo ID; hoặc `PriorityQueue` với duplicate policy rõ ràng.

## Wrong fixes
Retry `Add` không thay đổi kết quả. HashSet đơn thuần không giữ thứ tự ưu tiên. Random tie-breaker vi phạm deterministic ordering.

## Production implications
Theo dõi input unique IDs so với dispatched IDs, có audit trail cho alert không được phát.

## Trade-offs
SortedSet có O(log n) insert nhưng yêu cầu comparer total order. List + sort dễ hiểu hơn với batch, tốn O(n log n). Không thread-safe.

## Senior observation
Phải phân biệt business identity và priority ordering; định nghĩa rõ semantics khi cùng ID nhưng khác Severity.

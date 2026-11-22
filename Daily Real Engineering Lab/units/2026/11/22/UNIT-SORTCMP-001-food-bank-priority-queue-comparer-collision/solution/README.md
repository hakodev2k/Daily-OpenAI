# Reference solution — UNIT-SORTCMP-001

## Root cause
`SortedSet<T>` sử dụng kết quả `IComparer<T>.Compare` để xác định cả thứ tự lẫn tính tương đương. Comparer chỉ so Priority nên các Dispatch có cùng Priority bị xem là một phần tử; `Add` trả false và dữ liệu mất im lặng.

## Fix
Dùng thứ tự tổng: Priority giảm dần, rồi RequestId tăng dần theo `StringComparer.Ordinal`. Điều kiện: RequestId phải duy nhất và không thay đổi trong lúc nằm trong set. File tham khảo: [Program.cs](Program.cs).

## Trade-offs
Nếu cần nhiều yêu cầu trùng RequestId, tie-breaker phải là sequence/unique key thay vì RequestId. Nếu chỉ cần FIFO theo priority, cân nhắc `PriorityQueue<TElement,TPriority>` với sequence tăng dần; API và semantics khác SortedSet. Không sửa bằng cách tăng/giảm priority giả tạo, vì thay đổi nghiệp vụ.

## Verification
`pwsh -File scripts/verify.ps1` kiểm tra mã learner trong starter, không chạy mã solution. Đầu ra đúng phải giữ 4 RequestId duy nhất, ưu tiên 90 trước 70.

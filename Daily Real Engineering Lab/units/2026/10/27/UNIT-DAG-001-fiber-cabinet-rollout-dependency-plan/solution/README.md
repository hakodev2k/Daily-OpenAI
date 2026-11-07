# Reference Solution — SPOILER

**Chỉ đọc sau khi reproduce, ghi hypothesis và thử sửa `starter/Planner.cs`.**

## 1. Symptoms

NOC báo `SUCCESS` nhưng chỉ có 3/6 bước được lên kế hoạch; firmware ring A không thay đổi, và chạy lại cùng manifest cho kết quả giống hệt.

## 2. Evidence

`baseline` luôn cho thấy 3 bước đầu được chọn; các bước còn lại không xuất hiện. Manifest incident chứa một quan hệ phụ thuộc vòng giữa `flash-ring-a` và `healthcheck-ring-a`. Các log checksum và latency không giải thích số bước thiếu.

## 3. Root cause

Planner cũ coi việc **không còn bước sẵn sàng** là kết thúc bình thường, kể cả khi `pending` chưa rỗng. Sau đó luôn trả `Success=true`. Với prerequisite không tồn tại, cùng cơ chế trả partial success. Duplicate ID lại ném exception không có diagnostic có kiểm soát.

## 4. Why the fix works

`solution/Planner.cs` kiểm tra ID trùng và dependency không tồn tại, tính in-degree cho directed graph, chọn node có in-degree 0 theo thứ tự ổn định, cập nhật dependent và yêu cầu số node xuất ra bằng số node khai báo. Khi còn node chưa xử lý, graph chứa cycle và trả lỗi; không xuất bản kế hoạch một phần.

## 5. Verification

```powershell
./reproduce.ps1
./verify.ps1
dotnet run --project solution/Solution.csproj -- verify
```

Hai lệnh cuối chỉ đều PASS sau khi bạn sửa starter; baseline luôn chứng minh lỗi ban đầu. Kiểm tra 7 hành vi: graph hợp lệ, cycle, unknown dependency, duplicate ID, self-cycle, graph rỗng, deterministic order.

## 6. Alternative fixes

DFS với ba màu `unvisited/visiting/visited` cũng phát hiện cycle và tạo topological order, nhưng cần xử lý recursion depth và deterministic traversal. Kahn algorithm phù hợp cho việc giải thích số node còn kẹt và có thể triển khai iterative.

## 7. Wrong / tempting fixes

- Trả thành công ngay khi không còn node ready: che giấu rollout thiếu bước.
- Bỏ qua dependency không tồn tại: làm mất prerequisite an toàn.
- Thêm retry hoặc timeout: không sửa manifest vô nghiệm, chỉ tạo noise.
- Bỏ một edge trong cycle theo heuristic: thay đổi yêu cầu vận hành mà không có approval.
- Chỉ kiểm tra `Order.Count > 0`: không bảo đảm tính đầy đủ hoặc thứ tự.

## 8. Production implications

Validate toàn bộ manifest **trước** khi dispatch; ghi correlation ID, manifest version, số bước declared/planned, lý do reject. Không chạy một phần plan bị reject. Cấu hình rollout phải được version hóa, audit và review.

## 9. Trade-offs

Kahn algorithm có thời gian `O(V + E)` với queue thông thường; `SortedSet` cho deterministic tie-break thêm chi phí `O(log V)` mỗi thao tác. Dùng lexicographic order giúp test ổn định nhưng không nên nhầm với business priority; production cần chính sách priority độc lập.

## 10. Senior engineer should notice

Planner không phải executor. Một validated DAG chỉ chứng minh **khả năng sắp xếp**, không chứng minh firmware đã cài, thiết bị khỏe, hay rollback thành công. Cần trạng thái thực thi riêng, failure isolation, idempotency, authorization, checkpoint và giám sát tiến độ thực tế.

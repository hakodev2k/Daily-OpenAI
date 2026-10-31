# Reference Solution — chỉ xem sau khi reproduce và tự sửa

## 1. Symptoms
Running total ở reading 2 (cùng timestamp với reading 3) nhảy từ 100 lên 150 Wh thay vì 120 Wh; tổng cuối 190 Wh vẫn đúng nên kiểm tra chỉ cuối ngày không phát hiện.

## 2. Evidence
Dữ liệu theo reading ID: `100, 20, 30, 40` Wh. Starter trả `100, 150, 150, 190`; business contract yêu cầu `100, 120, 150, 190`. Có hai readings cùng `recorded_at`.

## 3. Root cause
`SUM(delta_wh) OVER (ORDER BY recorded_at)` sử dụng default window frame trong SQLite là `RANGE BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW`. Với `RANGE`, các rows cùng ORDER BY value là **peers**; frame của reading 2 đã chứa cả reading 3. `ORDER BY` ngoài cùng chỉ sắp xếp output, không thay đổi frame tính toán.

## 4. Why the fix works
Thay window clause bằng:
```sql
SUM(delta_wh) OVER (
    ORDER BY recorded_at, reading_id
    ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
)
```
`ROWS` tích lũy theo hàng của window order; `reading_id` tạo total order ổn định cho những timestamps bằng nhau.

## 5. How to verify
Sửa `starter/Program.cs` rồi chạy `.\verify.ps1`; phải có `VERIFY PASS` và `100, 120, 150, 190`. Đối chiếu bằng `dotnet run --project .\solution\SmartMeterLedger.csproj -- --verify` chỉ **sau** khi tự làm.

## 6. Alternative fixes
Có thể dùng `ORDER BY recorded_at, reading_id` với default `RANGE` khi khóa thứ hai duy nhất, nhưng explicit `ROWS` diễn đạt đúng contract per-row rõ hơn. Correlated subquery cũng có thể đúng nhưng thường khó tối ưu và bảo trì hơn.

## 7. Wrong / tempting fixes
- Chỉ thêm `ORDER BY reading_id` ở SELECT ngoài: không đổi window frame.
- `DISTINCT` hoặc `GROUP BY recorded_at`: làm mất độ hạt từng reading.
- Sửa readings trùng timestamp thành timestamp giả: làm sai dữ liệu nguồn.
- Chỉ assert tổng cuối ngày: che giấu regression ở các hàng giữa.

## 8. Production implications
Thêm integration tests với timestamps trùng nhau, đọc SQL execution plan và đánh giá index `(recorded_at, reading_id)` trên dữ liệu thật. Kiểm tra semantics khi có readings backfill hoặc dữ liệu đến muộn.

## 9. Trade-offs
Window ordering có thể cần sort; index phù hợp giúp giảm chi phí. `ROWS` phụ thuộc thứ tự xác định nên phải có tie-breaker. Nếu nghiệp vụ yêu cầu tổng theo timestamp thay vì từng reading, `RANGE` có thể đúng.

## 10. Senior engineer should notice
Phải định nghĩa rõ granularity của metric, uniqueness của ordering key, test peer rows, kiểm tra SQL dialect và không suy luận correctness chỉ từ tổng cuối.

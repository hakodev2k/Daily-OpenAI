# Reference Solution — chỉ xem sau khi đã tự điều tra

## 1. Symptoms
Latency summary có tail spike nhưng retained traces không chứa request chậm tương ứng.

## 2. Evidence
Simulator tạo 1000 requests và một số deterministic outliers. Starter giữ baseline sample trước khi outcome latency được biết; vì pattern sampling không giao với pattern outlier trong dataset này, `slow-kept=0` dù `slow-all>0`.

## 3. Root cause
Telemetry policy đang dùng quyết định kiểu head-based cho diagnostic evidence cần phụ thuộc vào outcome. Quyết định giữ/bỏ được đưa ra trước khi biết request nào trở thành slow outlier, nên rare tail requests có thể bị loại hết khỏi trace corpus.

## 4. Why the fix works
Reference implementation vẫn giữ baseline sample để quan sát traffic bình thường, đồng thời giữ request vượt diagnostic latency threshold sau khi outcome đã biết. Đây là mô phỏng local của ý tưởng tail/outcome-aware retention; production implementation phải phù hợp với tracing backend và cost budget thực tế.

## 5. How to verify
Áp dụng cùng policy vào `starter/Program.cs`, sau đó chạy `./verify.ps1`. PASS yêu cầu có slow trace và tổng traces vẫn nhỏ hơn tổng requests.

## 6. Alternative fixes
- Tạm thời tăng sampling trong incident window nếu cost cho phép.
- Dynamic sampling theo endpoint/tenant có rủi ro cao.
- Tail sampling ở collector/backend khi platform hỗ trợ.
- Giữ exemplars/correlation giữa metrics và traces để đi từ tail metric sang representative trace.

## 7. Wrong / Tempting Fixes
- Giữ 100% trace vĩnh viễn: có thể chẩn đoán được nhưng bỏ qua cost và ingestion constraints.
- Kết luận downstream khỏe chỉ vì sampled traces đều nhanh: dataset quan sát đã bị selection bias.
- Chỉ tăng logging: logs không tự sửa sampling blind spot và có thể tăng volume mà vẫn thiếu request cần tìm.

## 8. Production implications
Sampling là một phần của observability design, không chỉ là cost knob. Policy phải xuất phát từ câu hỏi điều tra cần trả lời, SLO, traffic volume, privacy và retention budget.

## 9. Trade-offs
Outcome-aware/tail retention cần buffering hoặc quyết định muộn hơn, tăng memory/collector complexity và có thể tăng ingestion. Baseline sampling rẻ hơn nhưng không đảm bảo rare incident evidence.

## 10. What a Senior engineer should notice
Dashboard và trace explorer có thể đại diện cho hai populations khác nhau. Trước khi suy luận từ telemetry, phải hiểu data-generation pipeline: instrumentation → sampling → export → ingestion → aggregation → query.
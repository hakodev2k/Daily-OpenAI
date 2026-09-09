# UNIT-CACHE-002 — Cache Boundary Decision

## Mục tiêu
Đưa ra quyết định caching cho một API đọc dữ liệu dựa trên latency, consistency, failure mode, topology và operational cost.

## Bối cảnh thực tế
Một profile API đọc từ SQL Server. p95 hiện còn trong SLO nhưng traffic dự kiến tăng mạnh. Team đang cân nhắc giữa không cache, in-process cache và distributed cache.

## Bạn cần làm gì
1. Đọc `docs/scenario.md`.
2. Hoàn thành `workspace/decision.md`.
3. So sánh ba phương án.
4. Nêu trade-offs và trigger để revisit decision.
5. Sau đó mới xem `solution/README.md`.

## Yêu cầu môi trường
Không cần runtime. Đây là Design Decision Lab.

## Chạy nhanh
Mở `docs/scenario.md` và hoàn thành decision worksheet.

## Cách reproduce vấn đề
Không áp dụng vì đây là design exercise.

## Những gì cần quan sát
Database headroom, read/write ratio, staleness budget, invalidation complexity, failure behavior và operational maturity.

## Quy tắc làm lab
Đọc constraints trước, ghi assumptions, ra decision, nêu trade-offs, rồi mới xem reference solution.

## Reference Solution
[Reference Solution](solution/README.md)

## Expected Results
Decision phải phù hợp với SLO và consistency requirements, đồng thời giải thích khi nào cần đổi phương án.

## Estimated Time
35–60 phút.

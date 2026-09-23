# UNIT-PG-001 — Case-Insensitive Index Boundary
## Mục tiêu
Điều tra một PostgreSQL lookup đúng chức năng nhưng latency tăng mạnh khi bảng lớn dần. Dùng execution plan để xác định nguyên nhân, sửa starter, rồi chứng minh bằng verification.
## Bối cảnh thực tế
SaaS customer directory cho phép tìm customer theo email không phân biệt hoa/thường. Khi dữ liệu ít endpoint nhanh; sau khi dữ liệu tăng, p95 tăng rõ rệt dù đã có index liên quan tới email.
## Bạn cần làm gì
1. Chạy starter và reproduce. 2. Thu thập EXPLAIN (ANALYZE, BUFFERS). 3. Ghi ít nhất 2 hypothesis. 4. Sửa starter/schema.sql và/hoặc starter/query.sql nhưng giữ lookup case-insensitive. 5. Chạy verify.ps1. 6. Sau đó mới xem solution.
## Yêu cầu môi trường
Docker Desktop/Engine có Docker Compose, PowerShell 7+, port 55432.
## Chạy nhanh
```powershell
./setup.ps1
./reproduce.ps1
```
## Cách reproduce vấn đề
reproduce.ps1 seed khoảng 200k customer, chạy lookup và in execution plan.
## Những gì cần quan sát
Scan strategy, số row phải xét, buffer activity, execution time, và việc query trả đúng dữ liệu có đồng nghĩa access path scale tốt hay không.
## Quy tắc làm lab
1. Reproduce trước. 2. Ghi hypothesis. 3. Thử fix. 4. Verify. 5. Chỉ sau đó mới xem solution.
## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)
## Reference Solution
⚠️ Spoiler: [Reference Solution](solution/README.md)
## Expected Results
Before: lookup đúng nhưng plan không đạt verification contract. After: lookup vẫn case-insensitive và có index-backed access path phù hợp.
## Estimated Time
45–60 phút.
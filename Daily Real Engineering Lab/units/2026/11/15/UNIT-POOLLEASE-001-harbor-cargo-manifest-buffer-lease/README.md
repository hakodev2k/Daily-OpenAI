# UNIT-POOLLEASE-001 — Manifest cảng biển bị thay đổi sau khi đọc

## Mục tiêu
Điều tra tính toàn vẹn dữ liệu giữa hai lượt ingest, thu thập evidence và sửa lifecycle của dữ liệu trả cho caller.

## Bối cảnh thực tế
Dịch vụ .NET xử lý manifest container. Request thành công nhưng bản manifest trước có thể thay đổi sau khi một tàu khác gửi dữ liệu, dù không có hai thread đồng thời.

## Bạn cần làm gì
Reproduce, ghi hai hypothesis, phân tích ownership, sửa `starter/Program.cs` và verify invariant: manifest trước không thay đổi sau request sau.

## Yêu cầu môi trường
.NET SDK 8+ và PowerShell. Không cần external package.

## Chạy nhanh
`pwsh ./run.ps1`

## Cách reproduce vấn đề
`pwsh ./reproduce.ps1` phải xác nhận symptom.

## Những gì cần quan sát
So sánh `Before` và `After` cho cùng manifest, exit code và reference lifetime.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
65 phút.

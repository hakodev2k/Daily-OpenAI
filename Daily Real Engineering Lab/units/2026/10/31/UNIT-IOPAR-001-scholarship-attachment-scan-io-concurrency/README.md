# UNIT-IOPAR-001 — Scholarship Attachment Scan: I/O Concurrency Budget

## Mục tiêu
Điều tra throughput thấp trong batch I/O, thu thập evidence, phân biệt CPU, latency và downstream capacity; đề xuất chính sách concurrency an toàn.

## Bối cảnh thực tế
Trường đại học phải quét 24 tệp hồ sơ học bổng qua partner scanner. Batch chậm dù CPU thấp, partner latency ổn định và chưa có 429. Đội vận hành đang cân nhắc scale CPU hoặc tăng mạnh số request đồng thời.

## Bạn cần làm gì
1. Reproduce hai cấu hình partner.
2. Ghi completed, peak, throttled, allClean, elapsedMs; đặt ít nhất ba hypothesis.
3. Chỉnh starter/Program.cs để đáp ứng cả hai cấu hình, không thay đổi simulator và acceptance tests.
4. Verify và so sánh với reference.

## Yêu cầu môi trường
.NET 8 SDK, Windows PowerShell 5.1+ hoặc PowerShell 7; không cần Azure, Docker hay database.

## Chạy nhanh
Từ thư mục unit:

    ./scripts/setup.ps1
    ./scripts/reproduce.ps1

## Cách reproduce vấn đề
    ./scripts/run.ps1
    ./scripts/reproduce.ps1

reproduce.ps1 kiểm tra triệu chứng của starter; verify.ps1 mới kiểm tra code người học đã sửa.

## Những gì cần quan sát
So sánh CAPACITY_8 và CAPACITY_1. Không dùng ngưỡng elapsedMs tuyệt đối vì tốc độ máy khác nhau. Xem docs/scenario.md và evidence/incident-timeline.md.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis tại workspace/my-investigation.md.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Chỉ mở sau khi tự điều tra](solution/README.md).

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
65 phút — L3 / D4.

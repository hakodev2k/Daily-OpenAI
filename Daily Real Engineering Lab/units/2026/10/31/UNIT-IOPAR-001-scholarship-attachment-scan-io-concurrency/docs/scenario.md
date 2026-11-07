# Incident — Scholarship Attachment Scan

## Business constraints
- 24 tệp phải được kiểm tra, không được bỏ sót.
- Partner A cho phép 8 request đồng thời; khi còn backlog, peak ít nhất 4 và không quá 8.
- Partner B chỉ cho phép 1 request đồng thời.
- Không chấp nhận HTTP 429 (simulator trả false) như quét thành công.
- Không chỉnh fake scanner, input hay acceptance assertions.

## Triệu chứng
Batch chậm dù CPU thấp và latency partner ổn định. Hai hướng đề xuất là scale CPU hoặc mở concurrency tối đa, nhưng chưa có bằng chứng.

## Local simulator
PartnerScanner giả lập I/O bằng Task.Delay 80ms, kiểm soát số in-flight và mô phỏng 429 khi vượt quota. Đây không phải benchmark production.

## Acceptance
CAPACITY_8=PASS: completed=24, peak [4,8], throttled=0, allClean=true.
CAPACITY_1=PASS: completed=24, peak=1, throttled=0, allClean=true.
scripts/verify.ps1 kiểm tra starter người học chỉnh sửa, không chạy solution.

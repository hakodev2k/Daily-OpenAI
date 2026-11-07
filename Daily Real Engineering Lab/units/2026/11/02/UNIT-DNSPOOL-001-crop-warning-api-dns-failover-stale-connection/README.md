# UNIT-DNSPOOL-001 — Crop warning API: DNS đã đổi nhưng client vẫn gửi đến vùng cũ

## Mục tiêu
Phân biệt DNS resolution, DNS cache và HTTP connection lifecycle; đề xuất chính sách failover cân bằng recovery time và connection overhead dựa trên bằng chứng.

## Bối cảnh thực tế
Một API cảnh báo sâu bệnh của hợp tác xã gọi dịch vụ dự báo qua HTTP keep-alive. Đội vận hành chuyển backend từ vùng east sang west tại giây 60. Dashboard DNS cho thấy west nhưng request tiếp theo vẫn trả 503. Lab dùng mô phỏng offline, không cần cloud.

## Bạn cần làm gì
1. Chạy mô phỏng, ghi thời điểm DNS lookup, connection creation, đích nhận request và status.
2. Đưa ra ít nhất hai giả thuyết và chứng cứ phân biệt.
3. Sửa chính sách trong `starter/Program.cs`; không sửa assertions `--verify`.
4. Chứng minh 2/2 request trước cutover và 5/5 request sau cutover thành công, tổng connection không quá 5.
5. Viết ADR ngắn về lựa chọn, alternatives, trade-offs và kế hoạch production verification.

## Yêu cầu môi trường
PowerShell 5.1+ hoặc PowerShell 7, .NET 8 SDK. `global.json` pin SDK 8.0.100 với latestFeature roll-forward; không cần NuGet package ngoài.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trên starter nguyên bản. Exit code 0 nghĩa là đã tái hiện **triệu chứng**, không phải đã sửa thành công.

## Những gì cần quan sát
Theo dõi `t`, `dnsActive`, `connected`, `status`, `connections`, `dnsLookups` ở từng mốc. Phân biệt tín hiệu DNS với hành vi request thực tế. Đây là model chính sách, không phải DNS resolver hay socket thật.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1` để kiểm tra **code bạn sửa**.
5. Chỉ sau đó xem reference solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) · [Reference code](solution/Program.cs). Chỉ xem sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). Reproduce và verify có mục đích khác nhau.

## Estimated Time
65 phút · L5 Design Decision / D4.

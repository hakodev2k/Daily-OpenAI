# UNIT-TSPAN-001 — Ticket hold chậm nhưng cảnh báo SLA lúc có lúc không

## Mục tiêu
Điều tra một sai lệch trong bộ phân loại độ trễ của gateway .NET: tái hiện kết quả, đối chiếu dữ liệu quan sát, nêu giả thuyết, sửa hợp đồng phân loại và chứng minh regression tests trên code của chính bạn.

## Bối cảnh thực tế
Hệ thống bán vé của một chuỗi nhà hát gửi yêu cầu giữ ghế sang đối tác. Gateway có ngưỡng cảnh báo phản hồi chậm là **750 ms** (chỉ vượt ngưỡng mới tính vi phạm). Trong đợt cao điểm, nhiều yêu cầu trên một giây không bị gắn cờ, trong khi một số yêu cầu ngắn hơn lại bị gắn cờ. Đối tác trả HTTP 200 và không có lỗi hàng đợi.

## Bạn cần làm gì
1. Chạy baseline, ghi lại ít nhất ba trường hợp bất thường và hai giả thuyết có thể kiểm chứng.
2. Đối chiếu mẫu đo với [incident evidence](evidence/incident.md), phân biệt dữ liệu quan sát với suy đoán.
3. Chỉ chỉnh sửa `starter/LatencyPolicy.cs`; không sửa baseline, fixture hoặc chương trình kiểm thử.
4. Chạy `verify.ps1` để kiểm tra code đã sửa, gồm ngưỡng bằng nhau, nhiều giây và dữ liệu không hợp lệ.
5. Đọc reference solution sau khi đã ghi lại kết quả điều tra.

## Yêu cầu môi trường
.NET SDK hỗ trợ `net8.0`, PowerShell 5.1+ hoặc PowerShell 7. Không cần Docker, database, cloud hoặc NuGet bên ngoài. Project chỉ dùng thư viện .NET có sẵn.

## Chạy nhanh
Từ thư mục unit:

```powershell
./setup.ps1
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

`REPRODUCTION_CONFIRMED` chỉ xác nhận lỗi của baseline cố định, không đánh giá bản sửa của bạn. Sau khi sửa, dùng `./verify.ps1`.

## Những gì cần quan sát
- Thời lượng thực tế và nhãn vi phạm SLA cho từng sample.
- Những cặp sample dài/ngắn được phân loại ngược kỳ vọng.
- Ranh giới chính xác của SLA 750 ms, các sample trên một giây và SLA lớn hơn một giây.
- Bằng chứng nào ủng hộ hoặc bác bỏ giả thuyết network jitter, queue backlog và lỗi tính toán.

Ghi lại điều tra tại [workspace/my-investigation.md](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ xem sau khi tự điều tra:** [Giải thích và trade-offs](solution/README.md) · [Reference code](solution/LatencyPolicy.cs).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
45 phút · L2 / D3 · Validation: `static-review` (chưa chạy build/tests trong lượt tạo).

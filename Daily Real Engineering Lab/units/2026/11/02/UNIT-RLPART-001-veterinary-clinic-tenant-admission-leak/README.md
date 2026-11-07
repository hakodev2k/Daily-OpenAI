# UNIT-RLPART-001 — Một phòng khám bị 429 vì lưu lượng của phòng khám khác

## Mục tiêu
Điều tra chính sách admission của API nhiều phòng khám bằng bằng chứng request-level và bảo toàn giới hạn theo hợp đồng.

## Bối cảnh thực tế
Nền tảng tiêm phòng thú y phục vụ nhiều phòng khám độc lập. Mỗi phòng khám được tối đa **2 yêu cầu trong cửa sổ 10 phút**, tính gộp hai endpoint. Sau hai request của Alpha, Beta bất ngờ nhận 429 ở request đầu. Alpha vẫn được nhận thêm một request trên endpoint khác.

## Bạn cần làm gì
Reproduce, ghi ít nhất ba hypothesis, thu thập evidence rồi sửa **chỉ** `starter/AdmissionPolicy.cs`. Không dùng header tự gửi làm danh tính đáng tin, không tăng quota.

## Yêu cầu môi trường
.NET SDK 8, PowerShell. Không cần Azure, Docker, database hay NuGet package bên ngoài.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` mô phỏng sáu request theo thứ tự cố định. Exit code 0 nghĩa là **đã reproduce triệu chứng**, không phải đã fix. Status HTTP chỉ là quyết định admission được mô phỏng, không phải network request thật.

## Những gì cần quan sát
So sánh kết quả từng request với quota theo phòng khám, thứ tự request và danh tính đã xác thực. Đọc [incident timeline](evidence/incident-timeline.md), ghi giả thuyết vào [workspace](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Chạy `./verify.ps1` kiểm tra chính starter đã sửa.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ xem sau khi tự điều tra.

## Expected Results
**Before:** Beta bị từ chối ở request đầu; Alpha được nhận thêm dù đã dùng hết quota. **After:** quota độc lập giữa phòng khám, dùng chung giữa hai endpoint trong cùng phòng khám.

## Estimated Time
65 phút. [Investigation guide](docs/investigation-guide.md).

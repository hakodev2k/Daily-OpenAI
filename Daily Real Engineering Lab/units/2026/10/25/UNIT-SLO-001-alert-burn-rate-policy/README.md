# UNIT-SLO-001 — Alert Policy: High Traffic Pages, Quiet Outage

## Mục tiêu
Thiết kế và kiểm chứng policy cảnh báo cho một dịch vụ .NET có lưu lượng thay đổi mạnh theo giờ. Phân biệt **alert có ích** với tín hiệu nhiễu; đánh giá bằng dữ liệu định lượng thay vì sửa ngưỡng theo cảm tính.

## Bối cảnh thực tế
Dịch vụ gửi thông báo lịch hẹn hoạt động bình thường nhưng on-call nhận cảnh báo liên tục vào giờ cao điểm. Ngược lại, trong khung giờ ít traffic, khách hàng báo không nhận được thông báo mà dashboard không phát cảnh báo. Hệ thống vẫn ghi nhận metrics và không có crash.

## Bạn cần làm gì
1. Chạy starter và ghi lại kết quả trên các tập dữ liệu cố định.
2. Từ [decision brief](docs/decision-brief.md), lập ít nhất ba giả thuyết, đề xuất SLI/SLO và chính sách paging có thể giải thích.
3. Sửa **chỉ** `starter/AlertPolicy.cs`; giữ nguyên interface `ShouldPage`.
4. Đảm bảo policy phân biệt sustained incident, healthy peak traffic, transient spike, recovery và no-traffic.
5. Chạy verification trên mã đã sửa, viết trade-off về low-traffic noise và missing telemetry.

## Yêu cầu môi trường
.NET SDK 8 và .NET 8 runtime, PowerShell; không cần Docker, Azure, database hoặc NuGet ngoài SDK. `global.json` pin SDK 8.0.100 với roll-forward.

## Chạy nhanh
Từ thư mục unit:

```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

Bash: `dotnet run --project starter/SloAlertLab.csproj -- reproduce`.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` trên starter chưa sửa. Exit code 0 có nghĩa là **đã quan sát đúng hai triệu chứng trái ngược**. Sau khi sửa, reproduce có thể không còn pass; đó là dự kiến.

## Những gì cần quan sát
- Cùng số lượng lỗi có luôn phản ánh cùng mức ảnh hưởng tới người dùng?
- Lưu lượng thay đổi có làm thay đổi tỷ lệ lỗi thực tế?
- Sự khác nhau giữa một spike ngắn và một sự cố kéo dài?
- Khi không có request, việc không có lỗi có đủ chứng minh dịch vụ khỏe mạnh?

Đọc [traffic evidence](evidence/traffic-samples.md) trước khi mở hints.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix và ghi lại quyết định kỹ thuật.
4. Chạy `./scripts/verify.ps1` trên **starter đã chỉnh sửa**.
5. Chỉ sau đó mới xem reference solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [solution/README.md](solution/README.md) và [reference code](solution/AlertPolicy.cs) — chỉ đọc sau khi tự giải.

## Expected Results
- Before: reproduce exit 0, verify exit khác 0; hai tình huống phân loại sai.
- After: verify exit 0 với 6 scenarios; quyết định paging có lý do, không bị chi phối đơn thuần bởi traffic volume.
- [Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
75 phút. Bài mở rộng Senior: [senior challenge](extensions/senior-challenge.md).

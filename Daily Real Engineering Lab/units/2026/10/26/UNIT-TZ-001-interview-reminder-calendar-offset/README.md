# UNIT-TZ-001 — Interview Reminders: 09:00 Local Time Drifts Across Calendar Boundaries

## Mục tiêu
Điều tra và cải tiến một thành phần lên lịch nhắc phỏng vấn: mỗi ứng viên phải nhận reminder cho cuộc hẹn **09:00 giờ New York**, kể cả khi các đợt phỏng vấn nằm ở những tuần khác nhau. Bảo toàn hợp đồng thời gian và chứng minh bằng regression checks.

## Bối cảnh thực tế
Cổng tuyển sinh đại học quốc tế tạo lịch reminder theo từng đợt. Đối tác lưu thời điểm thực thi dưới dạng UTC. Trong hai đợt thử nghiệm tháng 3 và tháng 10–11/2026, dashboard báo một số reminder lệch đúng một giờ; các ngày đầu đợt vẫn chính xác.

## Bạn cần làm gì
1. Chạy hệ thống mẫu, ghi lại ngày nào đúng/sai và ảnh hưởng tới ứng viên.
2. Nêu ít nhất hai hypothesis, phân biệt lỗi chuyển đổi thời gian với lỗi gửi email.
3. Chỉ sửa `starter/ScheduleBuilder.cs`, giữ nguyên hợp đồng 09:00 theo lịch địa phương.
4. Chạy verification và giải thích tại sao sửa đổi không làm lệch các ngày vốn đã đúng.

## Yêu cầu môi trường
- .NET SDK 8.0+; không cần NuGet bên thứ ba, Docker hoặc Azure.
- PowerShell 7+ (hoặc chạy các lệnh `dotnet` tương đương).
- Time-zone database của hệ điều hành/ICU được bật; không chạy globalization-invariant mode.

## Chạy nhanh
Tại thư mục unit:

```powershell
./scripts/run.ps1
./scripts/reproduce.ps1
```

Tương đương không dùng PowerShell:

```bash
dotnet run --project starter/ReminderScheduler.csproj -- demo
dotnet run --project starter/ReminderScheduler.csproj -- reproduce
```

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`. Lệnh **exit 0** khi chứng minh được tình trạng lỗi gốc (hai ngày đầu mỗi đợt đúng nhưng có ngày tiếp theo lệch). Không chỉnh sửa code trước lần reproduce đầu tiên.

## Những gì cần quan sát
So sánh từng `localDate`, `actualUtc`, `expectedUtc` trong console. Kiểm tra cả hai đợt thay vì chỉ ngày đầu; lưu kết quả trong `workspace/my-investigation.md`. Dữ liệu mô phỏng có trong `evidence/observations.md`.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Reference Solution](solution/README.md) — chỉ xem sau khi tự thử.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). `verify.ps1` chạy **learner-edited starter**, không chạy solution.

## Estimated Time
55 phút. Bài mở rộng: [Senior Challenge](extensions/senior-challenge.md).

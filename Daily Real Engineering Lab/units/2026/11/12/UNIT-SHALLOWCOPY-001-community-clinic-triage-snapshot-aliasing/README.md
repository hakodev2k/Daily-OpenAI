# UNIT-SHALLOWCOPY-001 — Snapshot bàn giao ca trực thay đổi sau khi lưu

## Mục tiêu
Refactor một đoạn C# phục vụ lưu bản ghi bàn giao ca trực. Tập trung vào quan sát identity, lifetime và bất biến dữ liệu trong một hệ thống đã vận hành.

## Bối cảnh thực tế
Một phòng khám cộng đồng lưu snapshot bảng phân loại bệnh nhân cuối ca. Khi nhân viên tiếp tục chỉnh bảng trực tiếp, bản ghi bàn giao cũ thay đổi dù không có thao tác sửa audit. Cơ sở dữ liệu và clock đều bình thường.

## Bạn cần làm gì
Chạy starter, lưu evidence của bản ghi trước/sau cập nhật, đề xuất hai giả thuyết, refactor starter để bản ghi đã lưu không đổi, rồi chạy verification. Giữ nguyên thứ tự và số lượng bệnh nhân.

## Yêu cầu môi trường
.NET SDK 8, PowerShell. Không cần database, internet hoặc package ngoài.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` để in snapshot tại thời điểm lưu và sau khi live board được cập nhật.

## Những gì cần quan sát
So sánh mã phân loại từng bệnh nhân trong snapshot trước/sau thao tác trên live board. Thu thập bằng chứng xem hai dữ liệu có thực sự độc lập.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/Program.cs`.
4. Verify bằng `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md)

## Expected Results
Trước: snapshot cũ không còn phản ánh thời điểm lưu. Sau: snapshot cũ ổn định trong khi bảng trực tiếp tiếp tục thay đổi.

## Estimated Time
45 phút.

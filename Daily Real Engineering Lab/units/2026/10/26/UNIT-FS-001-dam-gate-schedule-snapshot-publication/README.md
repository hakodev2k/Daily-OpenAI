# UNIT-FS-001 — Bảng lịch vận hành đập đọc phải snapshot đang cập nhật

## Mục tiêu
Reproduce và điều tra lỗi consistency tại ranh giới publish file, đề xuất hypothesis, sửa, rồi verify cả normal và failure path.

## Bối cảnh thực tế
Nhà máy thủy điện xuất lịch mở/đóng cửa xả dạng JSON. Console đọc lịch liên tục; đôi khi console gặp JSON không hợp lệ đúng lúc lịch được cập nhật, mặc dù phiên bản trước và sau đều hợp lệ.

## Bạn cần làm gì
1. Chạy harness và ghi lại `BEFORE`, `DURING`, `AFTER`.
2. Ghi ít nhất hai hypothesis vào [workspace](workspace/my-investigation.md).
3. Chỉ chỉnh `starter/GateSchedulePublisher.cs`; giữ nguyên chữ ký hàm.
4. Chạy `verify.ps1` kiểm tra snapshot nhất quán và failure path.
5. Sau khi tự sửa mới đọc reference.

## Yêu cầu môi trường
.NET SDK 8+, PowerShell. Không cần NuGet, Docker, Azure hay database. Dữ liệu test tạo trong thư mục tạm.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```
Linux/macOS: `dotnet run --project harness/GateScheduleHarness.csproj -- reproduce`.

## Cách reproduce vấn đề
`reproduce.ps1` kiểm tra nội dung reader nhận tại checkpoint có kiểm soát. Exit 0 = tái hiện được lỗi; không phải đã sửa xong.

## Những gì cần quan sát
- Bản trước và bản sau đều là JSON hợp lệ.
- Reader tại checkpoint có thể nhận nội dung khác cả hai bản hoàn chỉnh.
- Dữ liệu cuối cùng đúng không chứng minh được tính nhất quán của các lượt đọc đồng thời.
- Publish bị từ chối phải giữ nguyên bản cũ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Reference Solution](solution/README.md), chỉ mở sau khi đã tự thử.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
55 phút.

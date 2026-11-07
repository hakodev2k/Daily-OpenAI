# UNIT-STREAMPOS-001 — Bản ghi telemetry biến mất sau khi worker tiếp tục từ checkpoint

## Mục tiêu
Điều tra một sự cố mất bản ghi sau khi worker đọc feed theo từng batch và khởi động lại. Thu thập bằng chứng, sửa logic lưu tiến độ và chứng minh không bỏ sót hoặc xử lý lặp bản ghi trong fixture.

## Bối cảnh thực tế
Trạm mặt đất vệ tinh nhận file telemetry dạng từng dòng từ một hệ thống điều khiển cũ. Worker chỉ xử lý tối đa một số bản ghi mỗi lượt để tránh chiếm tài nguyên. Sau lần chạy đầu tiên, dashboard báo đã nhận một sự kiện; những lần chạy tiếp theo không nhận thêm dù file nguồn vẫn chứa các sự kiện chưa xử lý.

## Bạn cần làm gì
1. Chạy starter và tái hiện kết quả của ba lượt import nối tiếp.
2. Ghi ít nhất ba giả thuyết: lỗi nguồn, lỗi cursor, lỗi parser hoặc lỗi logic batch.
3. Kiểm tra bằng chứng: số dòng nguồn, giá trị checkpoint, kết quả từng batch, trạng thái sau restart.
4. Chỉ sửa `starter/FeedImporter.cs`; giữ nguyên fixture và acceptance harness.
5. Chạy verify; giải thích giới hạn của chiến lược checkpoint khi file bị thay thế hoặc truncate.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell (Windows hoặc PowerShell 7). Không cần database, Docker, Azure hay NuGet ngoài.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
./verify.ps1
```
Có thể dùng trực tiếp: `dotnet run --project starter/StreamCheckpointLab.csproj -- reproduce` hoặc `-- verify`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trên starter chưa chỉnh sửa. Exit code 0 nghĩa là triệu chứng được tái hiện đúng, **không** có nghĩa hệ thống đã được sửa. Sau đó chạy `./verify.ps1` để thấy acceptance hiện chưa đạt.

## Những gì cần quan sát
Đối chiếu tổng số dòng trong fixture với các bản ghi được trả về sau từng lượt. Ghi giá trị checkpoint trước và sau restart; đối chiếu với [incident evidence](evidence/incident.md). Không sửa fixture hoặc điều kiện kiểm thử để che lỗi.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi tự điều tra:** [Giải thích](solution/README.md) · [Reference code](solution/FeedImporter.cs)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
55 phút · L3 / D4. [Workspace điều tra](workspace/my-investigation.md).

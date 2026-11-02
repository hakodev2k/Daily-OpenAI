# UNIT-EFCTX-001 — Báo cáo kiểm định nước thải lỗi khi tổng hợp song song

## Mục tiêu

Tự reproduce một lỗi xuất hiện khi dashboard tổng hợp hai nhóm kiểm định cùng lúc, thu thập bằng chứng từ exception và thứ tự thực thi, phân biệt lỗi dữ liệu với lỗi phối hợp tác vụ, sửa code và chứng minh cả hai nhóm kết quả vẫn chính xác.

## Bối cảnh thực tế

Phòng kiểm định nước thải của quận phát hành báo cáo mỗi sáng: danh sách mẫu đang mở và danh sách mẫu đã hoàn tất. Một thay đổi để rút ngắn thời gian tạo báo cáo khiến tác vụ đôi khi thất bại trên môi trường có độ trễ I/O. Database vẫn có đủ ba mẫu; chạy từng truy vấn riêng thường thành công. Đội vận hành cần một cách tái hiện có kiểm soát thay vì dựa vào timing ngẫu nhiên.

## Bạn cần làm gì

1. Chạy bản starter, ghi lại triệu chứng và các thông điệp liên quan.
2. Ghi ít nhất ba hypothesis: kết nối SQLite, dữ liệu seed, vòng đời của thành phần truy vấn, hoặc thứ tự thực thi.
3. Đọc luồng ở `starter/InspectionReport.cs` và quan sát `checks/Program.cs` chỉ sau khi đã dự đoán hành vi.
4. Sửa **chỉ** `starter/InspectionReport.cs`; không sửa `checks/`, script hay dữ liệu kiểm thử.
5. Chạy verification, so sánh kết quả và giải thích chi phí tài nguyên, thread safety và lựa chọn chạy tuần tự/song song.

## Yêu cầu môi trường

.NET SDK 8 trở lên, PowerShell 5.1/7 và kết nối NuGet cho lần restore đầu tiên. Dùng EF Core SQLite 8.0.11 được pin trong project; không cần Docker, Azure hoặc database bên ngoài.

## Chạy nhanh

Tại thư mục unit:

```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

Sau khi sửa starter:

```powershell
./verify.ps1
```

Trên Linux/macOS có thể dùng `dotnet run --project checks/Checks.csproj -- reproduce` hoặc `-- verify`.

## Cách reproduce vấn đề

`reproduce.ps1` tạo SQLite database tạm với ba mẫu, sau đó dùng một điểm đồng bộ có kiểm soát để hai nhánh báo cáo thực sự chồng lấn. Script trả exit code 0 chỉ khi triệu chứng dự kiến được quan sát; lỗi bất ngờ hoặc không tái hiện được sẽ trả non-zero.

## Những gì cần quan sát

- Mã và loại exception, vị trí trong stack trace.
- Sự khác nhau giữa kết quả chạy từng truy vấn và chạy tổng hợp.
- Tại thời điểm nhánh thứ hai bắt đầu, nhánh thứ nhất đang ở trạng thái nào.
- Số mẫu trong database so với số mẫu có mặt trong báo cáo.
- Sau khi sửa: đúng hai mẫu Open và một mẫu Closed, không trùng hoặc mất mã.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ đọc sau khi tự điều tra và thử sửa:** [Reference Solution](solution/README.md).

## Expected Results

- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time

45–60 phút. Ghi bằng chứng vào [workspace/my-investigation.md](workspace/my-investigation.md).

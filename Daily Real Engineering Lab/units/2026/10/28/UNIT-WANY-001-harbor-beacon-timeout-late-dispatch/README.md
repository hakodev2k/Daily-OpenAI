# UNIT-WANY-001 — Cảnh báo phao cảng được gửi hai lần sau timeout

## Mục tiêu
Điều tra ranh giới giữa thời hạn chờ của request và vòng đời một thao tác I/O bất đồng bộ; chứng minh tính đúng đắn bằng fixture kiểm soát thứ tự thực thi thay vì dựa vào timing ngẫu nhiên.

## Bối cảnh thực tế
Gateway gửi cảnh báo thời tiết tới hệ thống phao báo hiệu của cảng. Khi đối tác phản hồi chậm, API trả timeout để hệ thống điều phối thử lại. Có trường hợp cùng một mã cảnh báo xuất hiện hai lần trong nhật ký phát đi, dù mỗi lần gọi API đều trả kết quả có vẻ hợp lý.

## Bạn cần làm gì
1. Chạy starter để quan sát một lần timeout, một lần retry và số thao tác gửi thực sự hoàn thành.
2. Ghi ít nhất hai giả thuyết, phân biệt lỗi ở phía gateway, transport và chính sách retry.
3. Chỉ chỉnh sửa `starter/AlertDispatcher.cs`. Giữ nguyên chữ ký public, bảo toàn kết quả `Delivered`/`TimedOut` và lỗi thực sự từ transport.
4. Chạy verify trên code bạn đã sửa; giải thích thêm vì sao cancellation chưa đủ bảo đảm exactly-once với đối tác thực.

## Yêu cầu môi trường
- .NET SDK 8.x, PowerShell 5.1+ hoặc PowerShell 7.
- Không cần Azure, database, NuGet ngoài SDK, internet hoặc đồng hồ chính xác. Fixture sử dụng các task gate nội bộ.

## Chạy nhanh
Từ thư mục unit:

~~~powershell
.\reproduce.ps1
~~~

Hoặc:

~~~bash
dotnet run --project starter/HarborAlertLab.csproj --configuration Release -- --reproduce
~~~

## Cách reproduce vấn đề
Giữ nguyên starter, chạy lệnh trên. Fixture chủ động cho một deadline kết thúc trước khi transport được mở khóa, sau đó retry cùng mã cảnh báo. Exit code 0 của `reproduce.ps1` chỉ xác nhận **triệu chứng starter**.

## Những gì cần quan sát
- API trả gì ở lần đầu và lần retry?
- Số `Active` của transport ngay sau khi lần đầu trả về.
- Tổng `Delivered` sau khi gate được mở.
- Thứ tự xảy ra giữa timeout, retry và completion của thao tác cũ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng `.\verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải thích và code tham chiếu](solution/README.md). Không mở trước khi tự điều tra.

## Expected Results
**Before:** `REPRODUCED`, lần timeout vẫn còn một send đang chạy và retry làm tổng hai lần delivery. **After:** `VERIFIED`, timeout không còn tác vụ treo, retry chỉ tạo một delivery, caller cancellation được truyền đúng và transport fault không bị che giấu.

## Estimated Time
30–45 phút. Ghi lại bằng chứng trong [workspace](workspace/my-investigation.md).

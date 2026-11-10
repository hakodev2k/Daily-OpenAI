# UNIT-TASKRACE-001 — Lệnh điều khiển phao cảng hoàn tất sau timeout

## Mục tiêu
Điều tra vòng đời bất đồng bộ của lệnh điều khiển khi timeout và phản hồi đến không đúng thứ tự.

## Bối cảnh thực tế
Trạm điều hành cảng gửi chế độ hoạt động cho phao tín hiệu. Một lệnh cũ được báo timeout, sau đó lệnh mới thành công; trạng thái cuối đôi khi không khớp lệnh mới nhất.

## Bạn cần làm gì
Chạy kịch bản, ghi nhận thứ tự sự kiện, xác định giả thuyết, thay đổi code để đảm bảo trạng thái cuối đúng với ý định điều khiển mới nhất.

## Yêu cầu môi trường
.NET SDK 8 trở lên, PowerShell trên Windows (hoặc terminal có dotnet).

## Chạy nhanh
```powershell
dotnet run --project ./starter/HarborLab.csproj
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Hoặc chạy trực tiếp project starter rồi quan sát dòng `mode after old ack`.

## Những gì cần quan sát
Ghi lại kết quả lệnh đầu, lệnh thứ hai, trạng thái trước và sau khi phản hồi trễ xuất hiện. Kiểm tra xem việc báo timeout có đảm bảo không còn tác động phụ nào hay không; đừng kết luận trước khi đối chiếu timeline.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md).

## Expected Results
Trước khi sửa: một chuỗi phản hồi trễ có thể khiến trạng thái cuối khác `ACTIVE`. Sau khi sửa: trạng thái cuối luôn `ACTIVE` trong kịch bản, kể cả khi phản hồi cũ đến muộn.

## Estimated Time
45 phút.

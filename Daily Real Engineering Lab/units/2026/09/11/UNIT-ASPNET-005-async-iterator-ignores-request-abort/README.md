# UNIT-ASPNET-005 — Client đã ngắt kết nối nhưng producer vẫn giữ tài nguyên

## Mục tiêu

Điều tra một production incident trong endpoint streaming của ASP.NET Core: client hủy request rất sớm nhưng công việc phía server vẫn tiếp tục, khiến tài nguyên giới hạn bị giữ lâu hơn dự kiến và request mới bắt đầu xếp hàng.

## Bối cảnh thực tế

Một endpoint export audit events dùng `IAsyncEnumerable<T>` để stream dữ liệu. Khi client tải bình thường, hệ thống ổn định. Trong giờ cao điểm, nhiều người dùng đóng tab hoặc hủy download giữa chừng. Dashboard cho thấy số request phía client đã kết thúc, nhưng số producer phía server và số slot tài nguyên đang sử dụng vẫn cao thêm một khoảng thời gian.

## Bạn cần làm gì

1. Chạy `reproduce.ps1` để quan sát symptom ban đầu.
2. Ghi ít nhất 2 hypothesis vào `workspace/my-investigation.md`.
3. Đọc code trong `starter/` và xác định vì sao tín hiệu cancellation từ consumer không dừng producer như mong đợi.
4. Sửa trực tiếp code trong `starter/`.
5. Chạy `verify.ps1` để xác nhận tài nguyên được giải phóng nhanh sau cancellation.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell
- Không cần database hay cloud service; lab dùng `FakeConnectionPool` cục bộ để mô phỏng tài nguyên giới hạn.

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/11/UNIT-ASPNET-005-async-iterator-ignores-request-abort"
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy 6 consumer đồng thời, mỗi consumer hủy sau một khoảng ngắn. Producer giữ một slot trong pool trong suốt vòng đời stream.

Script PASS khi starter thể hiện đúng incident: consumer đã phát tín hiệu cancellation nhưng producer vẫn còn active sau grace period.

## Những gì cần quan sát

- `CLIENT_CANCELLATIONS_SENT`
- `ACTIVE_PRODUCERS_AFTER_GRACE`
- `POOL_IN_USE_AFTER_GRACE`
- `WAITING_FOR_POOL_AFTER_GRACE`

Không coi việc client đã gọi `Cancel()` là bằng chứng rằng producer phía server đã dừng. Hãy theo dõi trạng thái producer và vòng đời tài nguyên.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi bạn đã reproduce và tự thử sửa.

- [Reference Solution](solution/README.md)

## Expected Results

### Trước khi sửa

- client cancellation được gửi cho cả 6 consumer
- sau grace period vẫn còn producer active
- pool vẫn còn slot bị giữ hoặc request đang chờ slot

### Sau khi sửa

- cancellation từ consumer đi vào producer
- producer đang chạy dừng sớm
- slot được trả lại nhanh
- sau grace period: `ACTIVE_PRODUCERS_AFTER_GRACE=0`, `POOL_IN_USE_AFTER_GRACE=0`, `WAITING_FOR_POOL_AFTER_GRACE=0`

Nếu kết quả không đúng, chạy `dotnet --info` và `dotnet restore starter/Lab.csproj` trước khi thử lại.

## Estimated Time

45–60 phút.
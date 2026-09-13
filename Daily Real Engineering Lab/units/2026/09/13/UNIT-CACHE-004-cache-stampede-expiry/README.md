# UNIT-CACHE-004 — Hot cache expiry làm downstream tăng tải đột biến

## Mục tiêu

Điều tra một incident nơi API vẫn trả dữ liệu đúng nhưng số lần gọi xuống data source tăng mạnh đúng thời điểm một key phổ biến hết hạn.

## Bối cảnh thực tế

Một Product Catalog API phục vụ flash sale có một SKU cực kỳ hot. Bình thường phần lớn request được phục vụ rất nhanh. Tuy nhiên, theo chu kỳ, dashboard cho thấy downstream dependency nhận một burst request lớn trong vài trăm milliseconds dù traffic phía client gần như không đổi.

Business impact: downstream có nguy cơ throttling, latency tăng và một sự cố nhỏ có thể lan thành outage.

## Bạn cần làm gì

1. Chạy starter và reproduce burst load.
2. Ghi lại hypothesis trước khi sửa.
3. Điều tra quan hệ giữa cache state, concurrent requests và số lần gọi downstream.
4. Sửa `starter/` để nhiều request đồng thời cho cùng key không khuếch đại tải xuống dependency.
5. Chạy `verify.ps1` để chứng minh behavior đã cải thiện mà dữ liệu trả về vẫn đúng.
6. Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7 hoặc Windows PowerShell

Không cần Redis, database, Docker hay cloud account.

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/13/UNIT-CACHE-004-cache-stampede-expiry"
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` khởi chạy nhiều request đồng thời cho cùng một hot key tại thời điểm cache không còn usable.

Script sẽ đọc output của starter và xác nhận rằng downstream bị gọi nhiều lần trong cùng một burst.

## Những gì cần quan sát

- `REQUESTS`
- `DOWNSTREAM_CALLS`
- `RESULTS_CONSISTENT`
- ứng dụng vẫn trả đúng payload nhưng dependency work bị nhân lên bao nhiêu lần

Đừng chỉ nhìn correctness của response. Hãy coi số lần thực thi expensive downstream operation là evidence chính.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi bạn đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Trước khi fix

- mọi request đều nhận được cùng business result
- số lần gọi downstream tăng gần với số concurrent requests
- vấn đề xuất hiện tại cache-miss/expiry boundary, không phải vì traffic tổng tăng đột biến

### Sau khi fix

- các request vẫn nhận cùng business result
- cùng một hot key chỉ gây khoảng 1 downstream reload cho một burst đồng thời
- fix không cần serialize toàn bộ request cho các key khác nhau

Không yêu cầu timing tuyệt đối vì scheduler khác nhau giữa các máy.

## Estimated Time

Khoảng **45–60 phút**.

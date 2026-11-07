# UNIT-CACHE-004 — Cache Stampede After Expiry

## Mục tiêu

Điều tra một read path có cache nhưng vẫn tạo ra burst lớn xuống nguồn dữ liệu khi một hot key hết hạn. Bạn cần dùng số liệu thực thi để xác định cơ chế gây amplification, sửa trên `starter/`, rồi chứng minh cache vẫn đúng về functional behavior và số lần tải nguồn giảm về mức mong đợi.

## Bối cảnh thực tế

Một pricing API phục vụ cùng một product rất thường xuyên. Bình thường latency thấp, nhưng cứ sau khi cache entry hết hạn thì database/downstream pricing service nhận một burst request gần như đồng thời. CPU của API không tăng đáng kể, nhưng dependency latency và connection usage tăng mạnh trong vài trăm mili-giây.

Lab dùng local in-memory simulator để tái tạo cơ chế này mà không cần Redis thật.

## Bạn cần làm gì

1. Chạy `./reproduce.ps1`.
2. Ghi lại `requestCount`, `sourceCalls` và hypothesis vào `workspace/my-investigation.md`.
3. Điều tra vì sao một hot key cache miss có thể tạo nhiều lần tải nguồn đồng thời.
4. Sửa trực tiếp code trong `starter/`.
5. Chạy `./verify.ps1`.
6. Chỉ sau đó mới xem `solution/`.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell
- Không cần Docker, Redis hoặc database ngoài

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script chạy 20 request đồng thời cho cùng một key khi cache đang trống và kiểm tra symptom.

## Những gì cần quan sát

- Tất cả request vẫn nhận đúng price.
- `requestCount` bằng `20`.
- `sourceCalls` lớn hơn nhiều so với số key khác nhau cần tải.
- Burst xuất hiện đúng tại thời điểm cache không có dữ liệu cho hot key.

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

> Spoiler: chỉ mở sau khi đã reproduce và tự thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before:
- 20 request trả đúng dữ liệu.
- Nhiều request cùng chạm nguồn dữ liệu cho cùng một key.
- `reproduce.ps1` pass khi symptom được tái tạo.

After:
- 20 request vẫn trả đúng dữ liệu.
- Nguồn dữ liệu chỉ bị tải một lần cho hot key trong burst này.
- `verify.ps1` pass trên code learner đã sửa trong `starter/`.

Nếu không reproduce được, chạy lại `./reproduce.ps1`; simulator có delay cố định để giữ cửa sổ concurrent miss đủ lớn trên máy local.

## Estimated Time

35–50 phút.

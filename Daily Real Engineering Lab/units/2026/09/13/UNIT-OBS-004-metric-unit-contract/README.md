# UNIT-OBS-004 — Dashboard latency lệch hàng trăm lần giữa hai dependency

## Mục tiêu

Điều tra một sai lệch telemetry khiến cùng một dashboard latency hiển thị hai cụm giá trị rất khác nhau dù các dependency có thời gian xử lý tương đương.

## Bối cảnh thực tế

Checkout service ghi latency của hai downstream dependency vào cùng một metric. Sau một thay đổi nhỏ ở instrumentation, dashboard bắt đầu xuất hiện một nhóm request có latency nhỏ bất thường, trong khi log nghiệp vụ và trải nghiệm thực tế không cho thấy dependency nhanh hơn tương ứng.

## Bạn cần làm gì

1. Reproduce hiện tượng.
2. Đọc các measurement được capture bởi `MeterListener`.
3. Ghi ít nhất 3 hypothesis trong `workspace/my-investigation.md`.
4. Xác định contract nào của metric đang bị vi phạm.
5. Sửa code trong `starter/` mà không đổi tên metric hay làm mất measurement.
6. Chạy verify và bảo đảm toàn bộ measurement nằm trong vùng hợp lý.
7. Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7 hoặc Windows PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/13/UNIT-OBS-004-metric-unit-contract"
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` build và chạy starter. Script chỉ PASS khi starter chứng minh được tập measurement hiện tại chứa giá trị bất thường theo contract của lab.

## Những gì cần quan sát

- Tên instrument và unit công bố.
- Các raw measurement được capture.
- Hai nhóm measurement có cùng ý nghĩa nghiệp vụ nhưng khác scale rõ rệt.
- Không có exception hay request failure nào giải thích sự khác biệt.

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

> Spoiler: chỉ mở sau khi đã tự điều tra và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

**Before**
- Starter chạy được.
- Có 4 measurement.
- Ít nhất một measurement nhỏ hơn 1 dù các simulated dependency latency đều ở mức hàng chục đến hàng trăm millisecond.
- Process trả exit code khác 0 để biểu diễn contract telemetry đang fail.

**After**
- Vẫn có đủ 4 measurement.
- Tất cả measurement nằm trong khoảng hợp lý cho dữ liệu mô phỏng của lab.
- Process trả exit code 0.

Nếu không reproduce được, chạy `dotnet --info`, sau đó `dotnet clean` và chạy lại script.

## Estimated Time

Khoảng 40 phút.

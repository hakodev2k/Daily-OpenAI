# UNIT-PERF-004 — Allocation Pressure in a Hot Formatting Path

## Mục tiêu

Điều tra một hot path xử lý batch có throughput giảm khi số record tăng, trong khi kết quả nghiệp vụ vẫn đúng. Dùng evidence về allocation và GC để xác định nơi cần tối ưu mà không thay đổi output contract.

## Bối cảnh thực tế

Một service tạo compact audit lines cho hàng chục nghìn events mỗi batch. Sau khi traffic tăng, CPU và GC activity tăng rõ rệt; latency của batch dao động mạnh dù không có I/O chậm hay exception. Team nghi ngờ nhiều nguyên nhân: logging, JSON input, collection growth hoặc formatting path.

## Bạn cần làm gì

1. Chạy starter và reproduce baseline.
2. Ghi hypothesis trước khi xem solution.
3. Quan sát allocated bytes và Gen0 collections được script báo cáo.
4. Sửa code trong `starter/` để giảm allocation đáng kể nhưng giữ nguyên output.
5. Chạy `verify.ps1` trên code bạn đã sửa.
6. Sau đó mới so sánh reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ khuyến nghị

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy workload deterministic nhiều vòng và in checksum, allocated bytes và GC collections. Chạy lại vài lần nếu máy đang có workload nền.

## Những gì cần quan sát

- Output checksum phải ổn định.
- Allocation tăng theo số record.
- Gen0 collections xuất hiện trong workload đủ lớn.
- Không giả định CPU cao tự động có nghĩa là cần parallelize.

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

[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results

**Before:** checksum đúng nhưng allocated bytes cao và Gen0 GC có thể tăng theo workload.

**After:** checksum giữ nguyên; allocation giảm đáng kể so với baseline trên cùng máy. Không dùng exact timing làm tiêu chí pass.

## Estimated Time

35–55 phút.
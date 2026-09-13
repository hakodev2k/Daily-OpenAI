# UNIT-PERF-003 — Regex validation làm request treo khi input gần khớp

## Mục tiêu

Điều tra một latency spike phụ thuộc input trong luồng validate dữ liệu và sửa theo cách giữ đúng business contract mà không chỉ che triệu chứng.

## Bối cảnh thực tế

Một API import customer bình thường xử lý nhanh. Tuy nhiên một số record có trường `ExternalCode` rất dài khiến một core chạy CPU cao và request validation mất thời gian bất thường. Không có database call hay downstream dependency nào chậm trong khoảng thời gian đó.

## Bạn cần làm gì

- Reproduce latency spike bằng input có sẵn.
- So sánh thời gian giữa input hợp lệ, input sai sớm và input gần khớp.
- Ghi ít nhất 2 hypothesis trong `workspace/my-investigation.md`.
- Sửa `starter/` để validation vẫn giữ business rule nhưng không còn tăng chi phí bất thường với input gần khớp.
- Chạy `verify.ps1`.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy ba nhóm input và kiểm tra rằng nhóm near-match chậm hơn rõ rệt so với baseline trên cùng máy.

## Những gì cần quan sát

- Input ngắn hợp lệ hoàn thành nhanh.
- Input sai ngay đầu chuỗi hoàn thành nhanh.
- Một input dài gần thỏa điều kiện có thể tiêu thụ CPU/time nhiều hơn rất lớn dù không có I/O.
- Functional result của input đó vẫn chỉ là `false`.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thu thập evidence từ timing và code path.
4. Thử fix trong `starter/`.
5. Verify.
6. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: inspect only after reproducing và thử fix.

[Reference Solution](solution/README.md)

## Expected Results

Before: near-match input có tỷ lệ thời gian cao bất thường so với baseline.

After: tất cả case trả về đúng kết quả và near-match không còn có latency spike ngoài ngưỡng verify.

## Estimated Time

45 phút.

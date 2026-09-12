# UNIT-EF-006 — Parallel queries thất bại trong request tổng hợp

## Mục tiêu

Điều tra một ASP.NET Core-style query service hoạt động ổn khi chạy từng truy vấn riêng lẻ nhưng thất bại khi tối ưu latency bằng cách chạy hai database operations song song.

## Bối cảnh thực tế

Một endpoint dashboard cần lấy `Orders` và `Alerts`. Sau thay đổi nhằm giảm response time, hai operations được khởi chạy đồng thời. Dưới request thật, endpoint thỉnh thoảng ném `InvalidOperationException` dù database không quá tải và từng query chạy độc lập đều thành công.

## Bạn cần làm gì

1. Chạy starter.
2. Reproduce lỗi bằng `./reproduce.ps1`.
3. Ghi ít nhất 2 hypothesis vào `workspace/my-investigation.md`.
4. Sửa code trong `starter/` mà không thay đổi business result.
5. Chạy `./verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.0.x
- PowerShell
- Không cần SQL Server/Docker; lab dùng EF Core InMemory để cô lập lifecycle/concurrency contract.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Exception xuất hiện khi hai operations overlap.
- Mỗi operation chạy riêng lẻ vẫn thành công.
- Scope/lifetime của object thực hiện database work.
- Việc thêm `Task.WhenAll` đã thay đổi concurrency model như thế nào.

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

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

- [solution/README.md](solution/README.md)

## Expected Results

Before: reproduce kết thúc thành công chỉ khi quan sát được failure contract dự kiến.

After: `verify.ps1` yêu cầu cả hai result đúng và không có concurrent-operation exception.

## Estimated Time

30–45 phút.
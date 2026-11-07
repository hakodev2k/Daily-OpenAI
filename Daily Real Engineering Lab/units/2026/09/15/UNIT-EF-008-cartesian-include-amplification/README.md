# UNIT-EF-008 — Cartesian Include Amplification

## Mục tiêu

Điều tra một read API EF Core trả dữ liệu đúng nhưng lượng dữ liệu đọc từ database và thời gian materialization tăng mạnh khi số collection liên quan tăng.

## Bối cảnh thực tế

Endpoint quản trị tải một `Order` cùng `Lines` và `Adjustments`. Dữ liệu nhỏ hoạt động bình thường. Với order lớn, endpoint chậm rõ rệt dù số entity nghiệp vụ không tăng tương ứng với số row mà truy vấn xử lý.

## Bạn cần làm gì

- Reproduce triệu chứng từ starter.
- Ghi hypothesis trước khi sửa.
- Quan sát số logical entity và số row shape được tạo bởi query.
- Sửa `starter/` để giữ nguyên dữ liệu nghiệp vụ nhưng giảm work amplification.
- Chạy `verify.ps1`.
- Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

Script chạy mô phỏng deterministic của query shape và kiểm tra rằng output nghiệp vụ đúng nhưng số row trung gian tăng mạnh khi hai collection cùng được tải.

## Những gì cần quan sát

- Số `Order`, `Line`, `Adjustment` thực tế.
- Số row trung gian cần xử lý.
- Tỷ lệ tăng khi mỗi collection lớn hơn.
- Dữ liệu trả về sau fix phải giữ nguyên.

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

> Reference Solution — chỉ xem sau khi reproduce và tự thử fix.

[solution/README.md](solution/README.md)

## Expected Results

Before: kết quả đúng nhưng work trung gian tăng theo tích kích thước các collection.

After: kết quả nghiệp vụ giữ nguyên và work được tách thành các tập tăng gần tuyến tính hơn trong scenario này.

## Estimated Time

45–60 phút.

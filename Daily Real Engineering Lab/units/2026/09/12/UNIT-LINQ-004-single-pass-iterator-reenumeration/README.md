# UNIT-LINQ-004 — Một pipeline dữ liệu, hai kết quả không nhất quán

## Mục tiêu

Điều tra một pipeline xử lý batch mà cùng một input lại tạo ra hai kết quả nghiệp vụ không nhất quán.

## Bối cảnh thực tế

Một reconciliation service đọc payment rows từ một ingestion adapter tuần tự. Tổng tiền của batch đúng, nhưng danh sách giao dịch cần kiểm tra rủi ro lại rỗng dù input có các khoản lớn. Không có exception.

## Bạn cần làm gì

1. Chạy starter và reproduce.
2. Ghi lại evidence và hypothesis trước khi sửa.
3. Theo dõi khi nào nguồn dữ liệu thực sự được đọc và mỗi phép tính nghiệp vụ quan sát bao nhiêu record.
4. Sửa `starter/` mà không hard-code kết quả.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.0.x
- PowerShell
- Không cần database, Docker hay dịch vụ bên ngoài

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Các dòng `read:*` trên stderr.
- `Total` và `HighRiskCount`.
- Thời điểm đường code tạo dữ liệu chạy so với lúc query được khai báo và lúc được dùng.
- Hai phép tính nghiệp vụ có thực sự quan sát cùng một tập record hay không.

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

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Before

- Với input mặc định, `Total=2900`.
- `HighRiskCount=0` dù input có hai khoản từ 1000 trở lên.
- Không có exception.

### After

- Input mặc định: `Total=2900`, `HighRiskCount=2`.
- Input `1000,50,2000,400`: `Total=3450`, `HighRiskCount=2`.
- Các phép tính nghiệp vụ dùng cùng một logical batch.

## Estimated Time

30–45 phút

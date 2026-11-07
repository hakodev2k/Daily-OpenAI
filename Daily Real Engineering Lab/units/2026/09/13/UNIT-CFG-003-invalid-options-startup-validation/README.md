# UNIT-CFG-003 — Cấu hình lỗi chỉ vỡ ở request đầu tiên

## Mục tiêu

Điều tra một service có thể khởi động và báo sẵn sàng, nhưng thao tác nghiệp vụ đầu tiên lại thất bại do một cấu hình triển khai không hợp lệ.

## Bối cảnh thực tế

Một reporting service vừa được đưa sang môi trường mới. Deployment pipeline thấy process start thành công nên tiếp tục đưa traffic vào instance. Vài giây sau, request tạo report đầu tiên trả lỗi. Cùng artifact đó vẫn chạy bình thường ở môi trường khác.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi lại evidence: thời điểm application báo đã start, loại exception, và thời điểm lỗi xuất hiện.
3. Đưa ra ít nhất hai hypothesis trước khi sửa.
4. Chỉnh sửa `starter/` để một deployment có cấu hình không hợp lệ bị từ chối trước khi application báo sẵn sàng nhận traffic.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell
- Không cần database, Docker hoặc cloud account

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script sẽ chạy đúng starter state và kiểm tra rằng application đã đi qua startup trước khi lỗi nghiệp vụ xuất hiện.

## Những gì cần quan sát

- Marker `APP_STARTED` có xuất hiện trước failure hay không.
- Exception xảy ra ở startup hay ở business operation.
- Giá trị cấu hình nào được đọc ở thời điểm operation chạy.
- Deployment pipeline có đủ tín hiệu để loại instance hỏng trước khi nhận traffic hay không.

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

- Process đi qua startup và in `APP_STARTED`.
- Operation đầu tiên thất bại sau đó.
- Exit code khác 0.

### After

- Deployment có cấu hình không hợp lệ bị từ chối trước marker `APP_STARTED`.
- Failure message mô tả rõ contract cấu hình bị vi phạm.
- Không cần hard-code giá trị production vào source code.

## Estimated Time

30–50 phút.
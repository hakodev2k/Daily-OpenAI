# UNIT-TLS-001 — TLS Certificate Name Boundary

## Mục tiêu
Điều tra một lỗi kết nối HTTPS xuất hiện sau khi endpoint nội bộ được chuyển sang hạ tầng mới.

## Bối cảnh thực tế
Một worker .NET gọi Partner API. Sau migration, TCP tới endpoint mới vẫn reachable nhưng HTTPS request thất bại trước khi API nhận request. Browser qua URL chuẩn vẫn hoạt động.

## Bạn cần làm gì
Reproduce bằng simulator, thu evidence, ghi hypothesis trong workspace, sửa cấu hình client ở `starter/client.json`, rồi chạy verify.

## Yêu cầu môi trường
- PowerShell 7+
- Không cần Azure hay Internet

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Simulator đọc certificate identity và request target từ các file trong `starter/`, sau đó mô phỏng bước validation liên quan đến identity.

## Những gì cần quan sát
- TCP reachability có thành công không.
- Host mà client dùng để gọi endpoint.
- Các DNS identities được certificate công bố.
- Failure xảy ra trước hay sau HTTP request.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/client.json`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
Before: network route reachable nhưng secure connection bị từ chối ở bước validation. After: client dùng endpoint identity phù hợp và request có thể tiếp tục tới HTTP layer.

Nếu không reproduce được, chạy PowerShell từ thư mục unit và kiểm tra `starter/client.json` cùng `starter/certificate.json` còn nguyên JSON hợp lệ.

## Estimated Time
45 phút.
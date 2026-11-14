# UNIT-HEADERBLEED-001 — Tenant header leak trong dịch vụ kiểm tra hiện trường

## Mục tiêu
Điều tra tính cô lập thông tin xác thực giữa hai yêu cầu HTTP chạy đồng thời, sử dụng bằng chứng từ một handler giả lập.

## Bối cảnh thực tế
Dịch vụ upload biên bản kiểm tra công trình phục vụ nhiều đơn vị hành chính. Khi hai đơn vị gửi đồng thời, một yêu cầu đôi lúc mang credential của đơn vị khác. Không có kết nối Internet trong bài lab.

## Bạn cần làm gì
Reproduce lỗi, ghi ít nhất hai giả thuyết, xác định boundary bị chia sẻ, sửa trong `starter/Program.cs`, chạy verify và đánh giá tác động production.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell. Không cần NuGet ngoài SDK.

## Chạy nhanh
Từ thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` để tạo overlap có kiểm soát giữa hai yêu cầu. Script thành công khi triệu chứng dự kiến được quan sát, không phải khi hệ thống đã được sửa.

## Những gì cần quan sát
Đối chiếu `tenant`, `actual`, `expected` trên hai bản ghi; xác định credential của yêu cầu nào bị sai. Thu thập bằng chứng trước khi thay đổi code.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix ở starter.
4. Chạy `./scripts/verify.ps1`.
5. Chỉ sau đó xem reference solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md) — chỉ mở sau khi tự thử.

## Expected Results
Trước sửa: `ISOLATION_FAIL`, reproduce exit 0, verify exit 1. Sau sửa: `ISOLATION_PASS`, verify exit 0. Nếu không reproduce được, kiểm tra SDK 8 và bảo đảm đang chạy starter gốc.

## Estimated Time
40 phút.

# UNIT-HEADERBLEED-001 — Lẫn thông tin xác thực giữa hai trường học

## Mục tiêu
Điều tra một sự cố isolation giữa các tenant khi ứng dụng .NET gọi dịch vụ đối tác; tái hiện, thu thập bằng chứng và sửa mà không thay đổi hành vi business.

## Bối cảnh thực tế
Một nền tảng xử lý ảnh kỷ yếu phục vụ nhiều trường học dùng chung HTTP client để gửi yêu cầu sang nhà in. Khi hai trường đồng thời xuất ảnh, đôi khi đối tác nhận credential không thuộc trường gửi yêu cầu. Dữ liệu có nguy cơ đi sai tenant.

## Bạn cần làm gì
Chạy bản starter, ghi lại tenant, credential kỳ vọng và credential mà fake partner thực nhận; nêu ít nhất hai giả thuyết; sửa phần code trong `starter/Program.cs`; chạy verification và đánh giá trade-off.

## Yêu cầu môi trường
PowerShell 7 hoặc Windows PowerShell, .NET SDK 8.0 (không cần NuGet ngoài hay kết nối mạng).

## Chạy nhanh
Từ thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` chạy hai tenant với lịch điều phối cố định và yêu cầu phải quan sát được sai lệch. Script trả exit 0 khi triệu chứng xuất hiện đúng dự kiến.

## Những gì cần quan sát
So sánh bảng expected/observed của hai request. Xem sự khác biệt giữa request logical identity và thông tin đối tác nhận. Không cần dịch vụ bên ngoài.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và phân tích](solution/README.md) — chỉ mở sau khi tự sửa.

## Expected Results
Trước: ít nhất một request không dùng credential của chính tenant. Sau: cả hai request luôn đúng và vẫn dùng chung HTTP client.

## Estimated Time
45–60 phút.

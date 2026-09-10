# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms

Ứng dụng đọc template thành công khi chạy từ project folder nhưng thất bại khi cùng DLL được launcher khởi động từ directory khác.

## Evidence

`Environment.CurrentDirectory` thay đổi theo launcher, trong khi template được copy vào application output. Path tương đối vì vậy trỏ tới location khác nhau.

## Root cause

Starter dùng `Path.Combine("templates", "invoice.txt")`. Đây là relative path và được resolve dựa trên process working directory, không phải vị trí application binary. Deployment launcher/service không có nghĩa vụ đặt working directory bằng artifact directory.

## Why the fix works

Reference solution anchor resource bằng `AppContext.BaseDirectory`, sau đó nối `templates/invoice.txt`. Anchor này gắn với application location nên không phụ thuộc directory mà process được launch từ đó.

## How to verify

Sửa `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Script build learner code, chuyển working directory sang temp folder rồi chạy chính starter DLL. Kết quả phải có `INVOICE=Invoice for ACME` và `LAB_VERIFY_PASS`.

## Alternative fixes

- Inject một explicit content-root/configured resource root vào component thay vì truy cập path trực tiếp.
- Embed resource vào assembly nếu resource thực sự immutable và phù hợp với deployment model.
- Với ASP.NET Core/Generic Host, dùng host/content-root abstraction khi resource thuộc application content contract.

## Wrong or misleading fixes

- `Set-Location` trong deployment script để ép working directory: có thể che symptom nhưng giữ implicit contract mong manh.
- Copy template vào mọi working directory có thể có: nhân bản artifact và khó vận hành.
- Hard-code absolute path theo máy production: phá portability và local reproducibility.
- Retry `File.ReadAllTextAsync`: không sửa sai path contract.

## Production implications

Working directory khác nhau giữa IDE, Windows Service, container entrypoint, scheduled task và deployment tooling là bình thường. Resource resolution phải có explicit anchor/contract.

## Trade-offs

`AppContext.BaseDirectory` đơn giản và phù hợp với resource deploy cùng binary. Explicit configured content root linh hoạt hơn khi resource được mount hoặc quản trị bên ngoài, nhưng thêm configuration contract và validation.

## What a Senior engineer should notice

Đây không chỉ là lỗi `File.Exists`. Root problem là deployment assumption không được biểu diễn thành contract. Khi refactor, cần xác định resource thuộc binary artifact, application content, configuration-managed data hay external storage; mỗi loại cần lifetime và location contract khác nhau.

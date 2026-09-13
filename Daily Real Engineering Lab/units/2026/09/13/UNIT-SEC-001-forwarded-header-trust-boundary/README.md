# UNIT-SEC-001 — Trusted Proxy Boundary và IP-based Authorization

## Mục tiêu

Điều tra một authorization boundary phụ thuộc vào client IP trong hệ thống chạy sau reverse proxy, thu thập evidence, xác định trust boundary đúng và sửa mà không phá legitimate proxied traffic.

## Bối cảnh thực tế

Một internal operations endpoint chỉ cho phép request từ một IP vận hành đã được allowlist. Production deployment đứng sau reverse proxy và ứng dụng cần xác định original client IP từ forwarding metadata.

Security review phát hiện một request đi trực tiếp từ mạng không được phép vẫn có thể được đánh giá là đến từ IP allowlist trong một số điều kiện. Không có dấu hiệu credential bị lộ.

## Bạn cần làm gì

1. Reproduce hành vi bằng starter simulator.
2. Ghi symptoms, evidence và ít nhất 3 hypotheses vào `workspace/my-investigation.md`.
3. Xác định trust boundary giữa direct client, reverse proxy và application.
4. Sửa learner-editable code trong `starter/`.
5. Chạy `verify.ps1` để kiểm tra cả attack case và legitimate traffic.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script mô phỏng một request có network peer không nằm trong allowlist nhưng mang forwarding metadata tuyên bố một client IP khác. Starter phải chứng minh endpoint vẫn bị cấp quyền sai.

## Những gì cần quan sát

- network peer thực sự của request
- forwarded client identity mà application nhìn thấy
- authorization decision cuối cùng
- legitimate request đi qua trusted proxy có còn hoạt động sau khi sửa hay không

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

> Spoiler: chỉ mở sau khi đã reproduce và tự sửa.

- [Reference Solution](solution/README.md)

## Expected Results

Before: request từ untrusted peer có thể được đánh giá như client allowlisted.

After: untrusted peer không thể thay đổi effective client identity bằng forwarding metadata, trong khi request đi qua trusted proxy vẫn giữ đúng original client identity.

## Estimated Time

45–60 phút

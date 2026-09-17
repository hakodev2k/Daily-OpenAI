# UNIT-SEC-001 — Forwarded Host Trust Behind a Reverse Proxy

## Mục tiêu
Điều tra một API account recovery chạy sau reverse proxy, nơi absolute URL được tạo từ thông tin request và đôi lúc chứa hostname không thuộc hệ thống.

## Bối cảnh thực tế
Ứng dụng gửi recovery link qua email. Production traffic bình thường đi qua reverse proxy, nhưng security review phát hiện có request tạo ra link trỏ đến một hostname lạ dù route, token và response đều hợp lệ.

## Bạn cần làm gì
1. Chạy starter và reproduce.
2. Ghi lại các request headers và absolute URL được tạo.
3. Đưa ra ít nhất hai hypothesis về trust boundary.
4. Sửa learner-editable code trong `starter/`.
5. Chạy `verify.ps1` để kiểm tra cả legitimate proxy request và request không đáng tin cậy.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell
- Không cần reverse proxy thật; lab dùng deterministic request simulator.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Host và forwarding-related headers của từng request.
- Absolute URL mà application tạo ra.
- Sự khác biệt giữa request được xem là đi qua proxy hợp lệ và request trực tiếp.
- Application hiện đang quyết định public origin từ dữ liệu nào.

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
[Reference Solution — spoiler](solution/README.md)

## Expected Results
Before: một request không thuộc trusted proxy path có thể ảnh hưởng hostname của generated recovery URL.

After: legitimate proxy request vẫn tạo đúng public URL; untrusted request không thể thay đổi public host và regression checks vẫn pass.

## Estimated Time
50 phút.
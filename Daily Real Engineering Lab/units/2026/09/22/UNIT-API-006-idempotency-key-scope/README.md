# UNIT-API-006 — Idempotency Key Scope

## Mục tiêu
Điều tra một lỗi retry trong multi-tenant command API: từng request riêng lẻ trông hợp lệ, nhưng một tenant đôi khi nhận kết quả của command thuộc tenant khác.

## Bối cảnh
Payment Command API hỗ trợ `Idempotency-Key` để client retry an toàn sau timeout. Store hiện tại giữ kết quả đã xử lý để tránh tạo payment hai lần.

## Nhiệm vụ
1. Chạy starter và reproduce triệu chứng.
2. Thu thập evidence từ output và code; viết ít nhất 2 hypotheses trong `workspace/my-investigation.md`.
3. Xác định identity boundary thực sự của một idempotent command.
4. Sửa code trong `starter/` để hai tenant có thể dùng cùng raw key mà không collision, đồng thời retry cùng tenant + operation + key vẫn replay đúng kết quả.
5. Chạy `./verify.ps1`.
6. Sau khi hoàn thành mới so sánh với `solution/README.md`.

## Constraints
- Không bỏ idempotency.
- Không generate key mới ở server để né collision.
- Không chỉ prefix ngẫu nhiên.
- Không hard-code tenant cụ thể.
- Fix phải giữ được semantics retry của cùng logical command.

## Chạy
```powershell
./run.ps1
```

## Reproduce
```powershell
./reproduce.ps1
```

## Verify
```powershell
./verify.ps1
```

## Expected symptom
Hai request khác tenant dùng cùng `Idempotency-Key` không tạo hai payment độc lập; request thứ hai replay kết quả đã cache của request thứ nhất.

## Hints
Mở theo thứ tự khi cần: `hints/hint-01.md`, `hint-02.md`, `hint-03.md`.

## Expected result sau fix
- Tenant A và Tenant B với cùng raw key tạo hai command độc lập.
- Retry của Tenant A với cùng operation + key replay đúng PaymentId của Tenant A.
- Không tăng số payment khi retry logical command đã hoàn tất.

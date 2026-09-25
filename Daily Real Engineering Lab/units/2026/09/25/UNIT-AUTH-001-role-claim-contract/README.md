# UNIT-AUTH-001 — Admin Token Still Gets 403

## Mục tiêu
Điều tra một lỗi authorization trong đó identity đã authenticated nhưng quyền truy cập không khớp với dữ liệu identity nhận được.

## Bối cảnh thực tế
Một internal operations API vừa chuyển sang token format do một identity provider mới phát hành. Login thành công, token của operator có dữ liệu role như mong đợi, nhưng endpoint dành cho Admin vẫn trả về quyết định deny. Các endpoint chỉ yêu cầu authenticated user vẫn hoạt động.

## Bạn cần làm gì
Reproduce lỗi, ghi lại evidence từ identity và authorization decision, đưa ra ít nhất 3 hypotheses, sửa code trong `starter/`, rồi chạy verification.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7 hoặc Windows PowerShell

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script phải xác nhận rằng Admin scenario hiện đang bị deny ngoài mong đợi.

## Những gì cần quan sát
- Identity có authenticated hay không.
- Danh sách claim type/value được tạo từ token payload.
- Kết quả kiểm tra quyền Admin.
- Reader scenario có bị nâng quyền ngoài ý muốn hay không.

Không thay đổi code trước khi ghi hypotheses vào workspace.

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
[Spoiler — chỉ xem sau khi đã thử sửa](solution/README.md)

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
50 phút.
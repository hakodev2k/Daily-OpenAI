# UNIT-REGEXLIT-001 — Đối soát nhãn container tại cảng

## Mục tiêu
Refactor chức năng tra cứu nhãn từ hệ thống .NET cũ; chứng minh kết quả bằng contract tests và kiểm tra false-positive.

## Bối cảnh thực tế
Bộ phận kho bãi quét mã nhãn container. Một số mã có dấu chấm, ngoặc hoặc dấu cộng. Hệ thống đôi khi trả thêm container không liên quan, hoặc không tìm thấy mã thực sự tồn tại.

## Bạn cần làm gì
Chạy starter, ghi hai giả thuyết và evidence. Chỉ sửa `starter/ContainerLookup.cs`, không sửa test fixture. Hợp đồng nghiệp vụ là **exact literal match, case-sensitive**.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell; không cần NuGet bên ngoài.

## Chạy nhanh
```powershell
pwsh ./scripts/setup.ps1
pwsh ./scripts/reproduce.ps1
# Sửa starter/ContainerLookup.cs
pwsh ./scripts/verify.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` thành công khi starter còn ít nhất một contract failure; `verify.ps1` chỉ thành công khi toàn bộ case pass.

## Những gì cần quan sát
So sánh kết quả mã chứa ký tự đặc biệt và mã thường; ghi lại kết quả và ngoại lệ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md), [Hint 2](hints/hint-02.md), [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Solution](solution/README.md).

## Expected Results
Trước: contract thất bại với ít nhất một nhãn; sau: exact-match, case-sensitive, missing-label và special-character tests đều pass.

## Estimated Time
40 phút.

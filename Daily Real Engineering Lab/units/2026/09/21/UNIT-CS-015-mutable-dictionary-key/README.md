# UNIT-CS-015 — Mutable Dictionary Key Lookup

## Mục tiêu
Điều tra một lỗi C# trong đó dữ liệu đã được thêm vào cache nhưng lookup sau đó báo không tồn tại.

## Bối cảnh thực tế
Một pricing service giữ rule theo composite key trong `Dictionary`. Sau khi metadata của key được cập nhật, monitoring cho thấy cache miss dù process chưa restart và entry chưa bị remove.

## Bạn cần làm gì
1. Chạy starter và reproduce triệu chứng.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Xác định contract nào của hash-based collection đang bị phá vỡ.
4. Sửa `starter/` mà không hard-code lookup result.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Entry count trước và sau thay đổi metadata.
- Kết quả `ContainsKey` với object key đang giữ.
- Những field nào tham gia equality/hash identity.
- Vì sao enumeration và key lookup có thể cho cảm giác mâu thuẫn.

## Quy tắc làm lab
Reproduce → evidence → hypothesis → fix → verify → compare solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results
Before: dictionary vẫn có một entry nhưng lookup theo key thất bại sau một thay đổi hợp lệ trong workflow.

After: key lookup ổn định qua workflow và một key nghiệp vụ khác không bị coi là cùng entry.

## Estimated Time
35 phút.
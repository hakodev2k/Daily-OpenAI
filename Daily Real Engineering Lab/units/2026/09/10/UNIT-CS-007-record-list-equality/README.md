# UNIT-CS-007 — Record collection equality

## Mục tiêu
Điều tra equality semantics của C# `record` khi value object chứa collection và sửa deduplication theo đúng business value.

## Bối cảnh thực tế
Webhook ingestion service dùng `HashSet<T>` để loại batch trùng. Hai payload có cùng tenant và cùng product IDs theo cùng thứ tự vẫn được giữ thành hai phần tử.

## Bạn cần làm gì
1. Chạy `./reproduce.ps1`.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Sửa code trong `starter/` để hai batch cùng sequence được coi là bằng nhau, nhưng khác thứ tự vẫn khác nhau.
4. Chạy `./verify.ps1`.
5. Sau đó mới xem `solution/README.md`.

## Yêu cầu môi trường
- .NET 8 SDK
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
- Hai batch có dữ liệu nghiệp vụ giống nhau.
- Hai collection được tạo độc lập.
- Equality hiện tại trả `False`.
- `HashSet` giữ cả hai batch.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- `hints/hint-01.md`
- `hints/hint-02.md`
- `hints/hint-03.md`

## Reference Solution
Xem `solution/README.md` sau khi tự thử.

## Expected Results
Trước fix: `HASHSET_COUNT=2`.
Sau fix: cùng sequence => equal, khác thứ tự => not equal, `HASHSET_COUNT=1`.

## Estimated Time
30–45 phút.

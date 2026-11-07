# UNIT-TEST-006 — InMemory Provider và Relational Semantics

## Mục tiêu

Điều tra một integration test đang xanh nhưng không kiểm chứng đúng database contract mà production phụ thuộc vào.

## Bối cảnh thực tế

Một account service yêu cầu email phải unique. Model có unique index, nhưng test hiện tại vẫn cho phép lưu hai account có cùng email.

## Bạn cần làm gì

- Chạy starter và reproduce hành vi.
- Ghi hypothesis về test database provider.
- Sửa test setup để kiểm chứng relational constraint.
- Chạy `verify.ps1`.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/14/UNIT-TEST-006-inmemory-relational-gap"
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Starter lưu được hai account trùng email.
- Không có database exception dù model khai báo unique index.

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

> Chỉ xem sau khi đã tự thử.

[Reference Solution](solution/README.md)

## Expected Results

Before: duplicate email được lưu thành công.

After: verification xác nhận relational unique constraint được thực thi.

## Estimated Time

30–45 phút.

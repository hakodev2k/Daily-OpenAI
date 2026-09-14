# UNIT-CS-012 — Invoice lệch 0.01 ở một số giá trị midpoint

## Mục tiêu

Điều tra một lỗi tính tiền chỉ xuất hiện ở một số giá trị nằm đúng midpoint khi hệ thống làm tròn monetary amount xuống 2 decimal places.

## Bối cảnh thực tế

Một batch invoice tax posting nhận các số tiền đã được tính chính xác bằng `decimal`. Phần lớn invoice khớp với hệ thống kế toán, nhưng một nhóm nhỏ lệch `0.01`. Business contract quy định midpoint phải được làm tròn theo chính sách của hệ thống kế toán, và kết quả phải nhất quán cho mọi invoice line.

## Bạn cần làm gì

- Chạy starter để reproduce mismatch.
- Ghi hypothesis trước khi sửa.
- Xác định đặc điểm chung của các input bị lệch.
- Kiểm tra policy làm tròn thực tế mà code đang sử dụng.
- Sửa `starter/` để toàn bộ contract cases pass.
- Chạy `verify.ps1` trên chính code bạn sửa.
- Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa `starter/`:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

Starter chạy một tập monetary contract cases gồm cả giá trị midpoint và non-midpoint, sau đó so kết quả tính toán với expected result từ accounting contract.

## Những gì cần quan sát

- Input nào pass và input nào fail.
- Sai lệch có luôn xảy ra hay chỉ ở một số midpoint.
- Kết quả actual và expected khác nhau theo pattern nào.
- Code có đang thể hiện rõ domain rounding policy hay đang dựa vào default behavior.

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

> Spoiler: chỉ xem sau khi bạn đã tự điều tra và thử fix.

[Reference Solution](solution/README.md)

## Expected Results

Before:
- Các non-midpoint case vẫn đúng.
- Một số midpoint case lệch `0.01` so với accounting contract.
- `reproduce.ps1` xác nhận symptom.

After:
- Tất cả contract cases đúng.
- Rounding policy được thể hiện explicit trong code.
- `verify.ps1` pass.

## Estimated Time

20–35 phút.

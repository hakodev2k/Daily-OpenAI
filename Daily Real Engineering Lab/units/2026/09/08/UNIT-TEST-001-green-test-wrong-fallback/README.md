# UNIT-TEST-001 — Test xanh nhưng fallback quote sai

## Mục tiêu

Điều tra một failure path nơi test suite hiện tại đều xanh nhưng hành vi quan sát được khi dependency timeout lại vi phạm business contract.

## Bối cảnh thực tế

Một checkout service lấy shipping quote từ carrier bên ngoài. Khi carrier timeout, checkout không được crash và phải trả về mức fallback đã được business phê duyệt để người dùng vẫn có thể tiếp tục đặt hàng. CI hiện đang xanh, nhưng một production probe cho thấy quote trả về khi timeout không đúng contract.

## Bạn cần làm gì

1. Chạy `reproduce.ps1` để chứng minh test suite xanh nhưng behavioral probe vẫn phát hiện sai lệch.
2. Ghi ít nhất 2 hypothesis trước khi sửa code.
3. Xác định test hiện tại đang chứng minh điều gì và điều gì nó chưa chứng minh.
4. Sửa code trong `starter/` và bổ sung regression coverage phù hợp.
5. Chạy `verify.ps1` để xác nhận cả test suite và business behavior đều đúng.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy test suite hiện tại trước, sau đó chạy một probe độc lập mô phỏng carrier timeout. Reproduction thành công khi test suite xanh nhưng probe ghi nhận fallback quote khác giá trị business yêu cầu.

## Những gì cần quan sát

- Test suite có pass hay không.
- Test hiện tại assert output behavior hay chỉ assert interaction / absence of exception.
- Giá trị quote khi carrier timeout.
- Sự khác biệt giữa “code path đã chạy” và “business contract đã được thỏa mãn”.

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

⚠️ Spoiler: chỉ mở sau khi đã tự reproduce, sửa và verify.

- [Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa: test suite hiện tại pass, nhưng probe cho thấy fallback quote không bằng `12.50`.

Sau khi sửa: test suite pass và probe trả về chính xác `12.50` khi carrier timeout.

## Estimated Time

35–50 phút.

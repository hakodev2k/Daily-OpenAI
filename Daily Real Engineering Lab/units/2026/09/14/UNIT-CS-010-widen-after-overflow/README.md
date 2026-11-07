# UNIT-CS-010 — Invoice total sai nhưng vẫn hợp lệ về kiểu dữ liệu

## Mục tiêu

Điều tra một calculation bug không ném exception nhưng trả về tổng tiền sai theo cách vẫn trông hợp lệ, sau đó sửa mà không che mất failure mode.

## Bối cảnh thực tế

Một billing batch tính tổng tiền theo `quantity * unitPriceCents`. Các invoice nhỏ đều đúng. Với một enterprise customer có volume lớn, tổng tiền trả về thấp hơn business expectation rất nhiều nhưng application không crash và giá trị vẫn là số dương.

## Bạn cần làm gì

- Reproduce invoice total sai bằng dữ liệu có sẵn.
- Ghi hypothesis trước khi xem solution.
- Xác định expression được evaluate bằng kiểu nào và ở thời điểm nào conversion xảy ra.
- Sửa `starter/` để kết quả đúng cho volume lớn và failure behavior rõ ràng nếu domain vượt range đã chọn.
- Chạy `verify.ps1`.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy một billing case lớn và xác nhận application trả về một total dương nhưng khác expected total.

## Những gì cần quan sát

- `quantity` và `unitPriceCents` đều riêng lẻ nằm trong range hợp lệ.
- Method return type đủ lớn để chứa expected total.
- Không có exception trong starter mặc định.
- Giá trị trả về vẫn dương nên có thể lọt qua validation hời hợt.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Inspect type của operands và expression.
4. Thử fix.
5. Verify.
6. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi đã thử fix.

[Reference Solution](solution/README.md)

## Expected Results

Before: total trả về là một số dương nhưng không bằng `4,900,000,000` cents.

After: learner-editable starter trả về đúng `4,900,000,000` cents và arithmetic boundary được biểu diễn rõ ràng.

## Estimated Time

30–40 phút.

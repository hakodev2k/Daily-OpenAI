# UNIT-CS-004 — Culture-sensitive identifier normalization

## Bối cảnh

Một service dùng mã SKU làm key để tra cứu dữ liệu đã cache. Code normalize key bằng `ToUpper()` mà không chỉ định semantics. Trên máy developer dùng culture mặc định, lookup có vẻ ổn; dưới một culture khác, cùng identifier có thể biến thành chuỗi khác và cache lookup thất bại.

## Chạy lab

```powershell
./run.ps1
```

## Reproduce

```powershell
./reproduce.ps1
```

Bạn phải thấy `LOOKUP=MISS` dù cùng logical SKU được dùng khi insert và lookup.

## Nhiệm vụ

1. Reproduce lỗi trước khi sửa.
2. Xác định vì sao behavior phụ thuộc process culture.
3. Sửa `starter/Program.cs` để identifier semantics không phụ thuộc culture của máy chạy.
4. Không đổi input để né lỗi.
5. Chạy `./verify.ps1`.

## Điều tra

Hãy phân biệt rõ hai bài toán:

- text dành cho người dùng, nơi culture có thể là một phần của semantics
- technical/business identifier, nơi comparison/normalization phải ổn định giữa môi trường

Nếu cần, xem `hints/`.

## Reference solution

Chỉ xem sau khi đã tự sửa: `solution/README.md`.

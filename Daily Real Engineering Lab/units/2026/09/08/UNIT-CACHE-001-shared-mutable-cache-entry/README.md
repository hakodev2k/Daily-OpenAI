# UNIT-CACHE-001 — Một request khuyến mãi làm request sau trả sai giá

## Mục tiêu

Điều tra một lỗi caching deterministic nơi request đầu tiên trả đúng giá khuyến mãi nhưng làm thay đổi kết quả của request bình thường chạy ngay sau đó.

## Bối cảnh thực tế

Một pricing service cache dữ liệu giá để giảm số lần đọc catalog. Trong chiến dịch khuyến mãi, request có cờ discount trả về 80 như mong đợi. Tuy nhiên request không discount ngay sau đó đôi khi cũng trả 80 thay vì giá gốc 100. Không có exception và cache hit rate vẫn cao.

## Bạn cần làm gì

1. Chạy `scripts/reproduce.ps1` để xác nhận symptom ban đầu.
2. Ghi ít nhất 2 hypothesis trước khi sửa code.
3. Quan sát output của hai request liên tiếp và dữ liệu còn lại trong cache.
4. Sửa code trong `starter/` mà không tắt cache.
5. Chạy `scripts/verify.ps1`.
6. Chỉ sau khi verify mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./scripts/run.ps1
```

## Cách reproduce vấn đề

```powershell
./scripts/reproduce.ps1
```

Script sẽ chạy starter state và xác nhận request có discount ảnh hưởng đến request bình thường chạy sau nó.

## Những gì cần quan sát

- Giá trả về cho request có discount.
- Giá trả về cho request bình thường ngay sau đó.
- Giá hiện còn được đọc từ cache sau cả hai request.
- Không có exception hay cache miss bất thường.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

⚠️ Spoiler — chỉ xem sau khi đã reproduce và tự thử fix.

- [Reference solution](solution/README.md)

## Expected Results

Trước khi sửa, reproduce phải xác nhận request bình thường thứ hai trả sai giá. Sau khi sửa, `verify.ps1` phải xác nhận request discount vẫn trả 80, request bình thường trả lại 100, và cached base price vẫn là 100.

## Estimated Time

25–40 phút.

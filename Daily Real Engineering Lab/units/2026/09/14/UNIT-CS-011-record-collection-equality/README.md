# UNIT-CS-011 — Record Collection Equality Breaks Deduplication

## Mục tiêu

Điều tra vì sao một cơ chế deduplication dùng `HashSet<T>` vẫn nhận hai request có cùng dữ liệu nghiệp vụ là hai phần tử khác nhau.

## Bối cảnh thực tế

Một background worker nhận yêu cầu export dữ liệu. Trước khi enqueue, service dùng `HashSet<ExportRequest>` để loại các request trùng nhau trong cùng batch. Sau khi DTO được chuyển sang `record`, team kỳ vọng value equality sẽ làm deduplication hoạt động tự nhiên. Tuy nhiên hai request nhìn giống hệt nhau vẫn tạo hai export jobs.

## Bạn cần làm gì

- Chạy `reproduce.ps1`.
- Quan sát số phần tử trong `HashSet` và dữ liệu của hai request.
- Ghi ít nhất hai hypothesis trước khi sửa.
- Xác định phần nào trong equality contract không phản ánh đúng business identity.
- Sửa `starter/` để hai request có cùng `CustomerId` và cùng ordered column set được xem là cùng một value.
- Đảm bảo request khác column vẫn được xem là khác.
- Chạy `verify.ps1`.
- Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa starter:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy starter và xác nhận batch hiện tại chứa hai entry dù hai request có cùng business data.

## Những gì cần quan sát

- Hai `ExportRequest` được tạo từ hai collection instance khác nhau.
- Nội dung từng collection là giống nhau.
- `HashSet<ExportRequest>.Count` không khớp kỳ vọng nghiệp vụ.
- Equality và hash code của object composite phụ thuộc vào semantics của từng member.

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

> Chỉ xem sau khi đã tự thử fix.

[Reference Solution](solution/README.md)

## Expected Results

Before:
- Hai request có cùng business data tạo ra `dedupe-count:2`.

After:
- Hai request tương đương tạo ra một entry.
- Một request có column khác vẫn tạo entry riêng.
- `verify.ps1` pass.

## Estimated Time

30–45 phút.

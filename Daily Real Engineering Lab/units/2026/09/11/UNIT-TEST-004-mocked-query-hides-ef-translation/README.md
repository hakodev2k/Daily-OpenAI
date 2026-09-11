# UNIT-TEST-004 — Mocked Query Hides EF Translation Failure

## Mục tiêu

Điều tra một regression mà test double dựa trên `IQueryable<T>` báo xanh nhưng cùng query lại không chạy được qua EF Core relational provider. Mục tiêu là cải thiện cả implementation lẫn chiến lược test để lỗi loại này bị phát hiện trước production.

## Bối cảnh thực tế

Một service tìm kiếm sản phẩm đã có unit test từ lâu. Test chạy rất nhanh và luôn xanh. Sau một thay đổi nhỏ về normalization, endpoint production bắt đầu trả lỗi khi thực hiện query trên database. Dữ liệu đầu vào và business rule không thay đổi.

## Bạn cần làm gì

1. Reproduce sự khác biệt giữa đường chạy test-like và đường chạy relational provider.
2. Ghi lại evidence và ít nhất hai hypothesis trước khi sửa.
3. Sửa code trong `starter/` để cùng business behavior chạy được ở cả hai đường.
4. Chạy `verify.ps1`.
5. Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell
- Không cần SQL Server/Docker; lab dùng SQLite in-memory.

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/11/UNIT-TEST-004-mocked-query-hides-ef-translation"
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy cùng implementation theo hai execution path:

- một path dùng dữ liệu in-memory qua `IQueryable<T>`
- một path dùng EF Core SQLite relational provider

Script chỉ PASS khi path đầu thành công còn path thứ hai thể hiện đúng failure ban đầu.

## Những gì cần quan sát

- Hai path nhận cùng dữ liệu và cùng search term.
- Path test-like trả đúng sản phẩm mong đợi.
- Path relational không hoàn thành query thành công.
- So sánh nơi query được thực thi và provider đứng sau `IQueryable<T>`.

Không kết luận root cause chỉ từ exception text; hãy giải thích vì sao test hiện tại không bảo vệ production behavior.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi bạn đã thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Trước khi sửa

- test-like path thành công và tìm thấy `Acme Widget`
- relational path thất bại trước khi materialize kết quả
- failure là deterministic và không phụ thuộc timing

### Sau khi sửa

- cả hai path đều tìm thấy đúng một sản phẩm
- `verify.ps1` exit code `0`
- không chuyển toàn bộ dữ liệu về memory để né query provider

Nếu reproduce không đúng, chạy `dotnet --info`, sau đó `dotnet restore starter/Lab.csproj` và thử lại.

## Estimated Time

35–55 phút.
# UNIT-EF-003 — Filtered Include Returns More Rows Than the Filter

## Mục tiêu

Điều tra một EF Core read path trong đó kết quả navigation collection không khớp với filter được viết trong query, thu thập evidence từ `ChangeTracker`, xác định boundary gây ra hành vi và sửa theo cách phù hợp với read-model semantics.

## Bối cảnh thực tế

Một màn hình hỗ trợ khách hàng cần hiển thị chỉ các order đang `Active`. Trên môi trường đơn giản, query trông đúng. Nhưng trong một request workflow có bước kiểm tra lịch sử trước đó, UI đôi lúc vẫn nhận cả order `Cancelled` dù câu query sau đó dùng filtered `Include`.

Business impact: nhân viên support có thể nhìn thấy dữ liệu không thuộc tập kết quả mong muốn và đưa ra quyết định sai khi xử lý khách hàng.

## Bạn cần làm gì

1. Chạy `./reproduce.ps1`.
2. Ghi lại SQL/query intent và entity state đang tồn tại trước query cuối.
3. So sánh filter trong LINQ với nội dung thực tế của `Customer.Orders`.
4. Đưa ra ít nhất hai hypothesis trước khi sửa.
5. Sửa code trong `starter/` mà không thay đổi business rule "chỉ Active".
6. Chạy `./verify.ps1`.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Không cần SQL Server, Docker hoặc cloud service; lab dùng SQLite in-memory

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

Script build và chạy starter theo chế độ reproduction. Một reproduction hợp lệ phải chứng minh navigation collection cuối cùng chứa nhiều item hơn tập được phép bởi business filter.

## Những gì cần quan sát

- số entity đang được track trước query cuối
- trạng thái của các `Order` trong `ChangeTracker`
- giá trị `Status` thực tế trong `Customer.Orders`
- sự khác biệt giữa query intent và object graph nhận được

Không thay filter thành một điều kiện khác chỉ để làm test xanh.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thu evidence từ tracking state.
4. Thử fix trong `starter/`.
5. Verify.
6. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

> **Spoiler:** chỉ mở sau khi bạn đã tự reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa:

- query cuối có filter `Status == "Active"`
- object graph trả về vẫn chứa ít nhất một order không phải `Active`
- evidence cho thấy context đã có tracked entity từ bước trước

Sau khi sửa:

- `Customer.Orders` chỉ chứa `Active`
- business behavior không phụ thuộc vào những entity đã được load trước đó
- `verify.ps1` pass ổn định

## Estimated Time

35–55 phút.

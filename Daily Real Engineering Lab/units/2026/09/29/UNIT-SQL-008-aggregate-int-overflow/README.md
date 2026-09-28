# UNIT-SQL-008 — Settlement report chỉ lỗi khi tổng tiền đủ lớn

## Mục tiêu

Điều tra một lỗi SQL Server chỉ xuất hiện khi dữ liệu production vượt một ngưỡng nhất định, dù từng record riêng lẻ đều hợp lệ.

Bạn cần reproduce lỗi, thu thập evidence về kiểu dữ liệu và giá trị biên, sửa query trong `starter/query.sql`, rồi verify rằng kết quả đúng với dataset lớn mà không thay đổi dữ liệu mẫu để né lỗi.

## Bối cảnh thực tế

Một settlement job tổng hợp `AmountCents` theo ngày cho merchant. Trong nhiều tháng query chạy bình thường. Sau khi volume tăng, một merchant lớn bắt đầu làm job fail với SQL Server error 8115.

Các dòng settlement riêng lẻ vẫn đọc được, không có giá trị âm bất thường và không có record nào vượt giới hạn của column hiện tại.

Business cần tổng chính xác để đối soát cuối ngày. Không được bỏ record, chia nhỏ dữ liệu thủ công hay trả partial result.

## Bạn cần làm gì

1. Chạy setup và reproduce.
2. Ghi lại error number, kiểu dữ liệu của `AmountCents`, từng giá trị mẫu và tổng toán học mong đợi.
3. Đưa ra ít nhất hai hypothesis trước khi sửa.
4. Sửa duy nhất query learner-editable trong `starter/query.sql`.
5. Kết quả phải là `3300000000`.
6. Chạy `verify.ps1`.
7. Sau đó mới xem reference solution và Wrong Fixes.

## Yêu cầu môi trường

- Windows với SQL Server LocalDB hoặc một SQL Server local/dev instance
- `sqlcmd` có trong PATH
- PowerShell
- Mặc định dùng `(localdb)\MSSQLLocalDB`
- Có thể đặt `LAB_SQL_SERVER` để dùng instance khác với Windows Authentication

## Chạy nhanh

~~~powershell
./setup.ps1
./reproduce.ps1
~~~

Sau khi sửa `starter/query.sql`:

~~~powershell
./verify.ps1
~~~

## Cách reproduce vấn đề

~~~powershell
./reproduce.ps1
~~~

Script sẽ reset dataset rồi chạy đúng starter query. Reproduction chỉ PASS khi SQL Server trả error 8115 như dự kiến.

## Những gì cần quan sát

Thu thập các fact sau trước khi thay query:

- kiểu SQL của column nguồn
- giá trị của từng row
- tổng toán học mong đợi
- error number từ SQL Server
- query hiện tại có aggregate trên expression kiểu gì

Không đổi schema hoặc giảm dataset chỉ để làm lỗi biến mất.

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

[Spoiler — chỉ xem sau khi đã reproduce và tự thử sửa](solution/README.md)

## Expected Results

- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time

25–40 phút.

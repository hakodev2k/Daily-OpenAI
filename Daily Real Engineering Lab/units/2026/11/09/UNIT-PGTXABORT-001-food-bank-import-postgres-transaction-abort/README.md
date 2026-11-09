# UNIT-PGTXABORT-001 — Import quyên góp: một bản ghi trùng làm hỏng cả transaction

## Mục tiêu
Điều tra một batch import có kết quả bất thường, chứng minh bằng SQLSTATE và thiết kế cách xử lý một dòng lỗi mà không làm mất các dòng hợp lệ.

## Bối cảnh thực tế
Hệ thống food bank nhận file đóng góp mỗi đêm. Một lần import có ba khoản đóng góp, trong đó có một mã đã tồn tại. Dashboard báo lỗi cho cả những khoản còn lại dù các mã đó hợp lệ. Bộ phận tài chính yêu cầu không ghi trùng, không ghi thiếu, và phải có đối soát.

## Bạn cần làm gì
Chạy script trong PostgreSQL cục bộ, thu thập output và SQLSTATE, lập ít nhất hai hypothesis, rồi sửa **starter/import.sql**. Không sửa fixture hoặc script kiểm tra.

## Yêu cầu môi trường
Docker Desktop với Docker Compose và PowerShell 7; hoặc PostgreSQL 16 cùng `psql`. Không cần Azure hay dịch vụ trả phí.

## Chạy nhanh
Từ thư mục unit:

```powershell
docker compose up -d
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` khởi tạo fixture và chạy phiên bản ban đầu; quan sát dòng lỗi thứ hai và các thông báo tiếp theo. Nếu dùng psql trực tiếp: `psql -X -v ON_ERROR_STOP=0 -h localhost -U lab -d lab -f starter/import.sql` sau khi nạp fixture.

## Những gì cần quan sát
Đối chiếu thông báo của từng câu lệnh, SQLSTATE, số lượng hàng trong bảng và mã đóng góp sau khi transaction kết thúc. Ghi nhận lệnh nào bị ảnh hưởng dù dữ liệu đầu vào của nó hợp lệ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/import.sql`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Xem lời giải sau khi tự điều tra](solution/README.md).

## Expected Results
Trước: có lỗi, không có đủ hai khoản hợp lệ mới. Sau: các khoản hợp lệ được ghi đúng một lần, mã trùng không tạo thêm bản ghi, không để transaction thất bại âm thầm.

## Estimated Time
65 phút.

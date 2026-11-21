# UNIT-LINQSNAP-001 — Báo cáo kiểm toán thay đổi sau khi đã chuẩn bị xuất dữ liệu

## Mục tiêu
Điều tra tính ổn định của báo cáo .NET khi người dùng tiếp tục chỉnh bộ lọc hoặc dữ liệu nguồn sau khi chuẩn bị export.

## Bối cảnh thực tế
Hệ thống hỗ trợ cộng đồng chuẩn bị danh sách khoản giải ngân để đối soát. Một bản xem trước đã được tạo, nhưng số dòng và tổng tiền có thể khác nhau giữa hai lần đọc, khiến người kiểm toán không thể xác nhận dữ liệu.

## Bạn cần làm gì
Chạy chương trình, lưu bằng chứng cho từng case, lập ít nhất hai hypothesis và sửa **starter/Program.cs** mà không thay đổi contract của các test.

## Yêu cầu môi trường
.NET SDK 8; PowerShell; không cần database hay secret.

## Chạy nhanh
```powershell
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` trước khi sửa. Script xác nhận có case thất bại. Ghi lại tên case và thay đổi đầu vào tương ứng.

## Những gì cần quan sát
So sánh ID, số lượng bản ghi và tổng tiền giữa các lần truy xuất kết quả đã chuẩn bị. Phân biệt việc thay đổi bộ lọc với việc thay đổi danh sách nguồn.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix.
4. Verify.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md)

## Expected Results
Trước sửa: ít nhất một case FAIL; sau sửa: cả năm case PASS, exit code 0.

## Estimated Time
40 phút.

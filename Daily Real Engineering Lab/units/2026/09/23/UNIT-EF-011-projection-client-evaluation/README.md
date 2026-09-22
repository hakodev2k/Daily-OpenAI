# UNIT-EF-011 — Projection Boundary Under Load

## Mục tiêu
Điều tra một read endpoint có kết quả đúng nhưng chi phí dữ liệu và memory tăng mạnh khi bảng lớn hơn.

## Bối cảnh thực tế
Customer Search API chỉ trả một danh sách nhỏ khách hàng APAC. Sau khi dữ liệu tăng, endpoint tạo nhiều allocation và lượng dữ liệu đọc từ database lớn hơn nhiều so với response.

## Bạn cần làm gì
Reproduce, quan sát SQL/log và số lượng object được đưa vào process, ghi hypothesis, sau đó sửa `starter/Program.cs` để giữ nguyên kết quả nghiệp vụ nhưng giảm dữ liệu không cần thiết.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell

## Chạy nhanh
`./run.ps1`

## Cách reproduce vấn đề
`./reproduce.ps1`

## Những gì cần quan sát
- SQL được EF Core phát ra.
- Số record được materialize so với số record response cần.
- Các cột được database trả về.
- Việc thay đổi kích thước dataset ảnh hưởng thế nào đến work phía application.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/Program.cs`.
4. Chạy `verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results
Before: response đúng nhưng query/application xử lý nhiều dữ liệu hơn cần thiết. After: vẫn trả tối đa 20 khách APAC, trong khi query được thu hẹp theo nhu cầu response trước khi dữ liệu đi vào application.

## Estimated Time
45–60 phút.
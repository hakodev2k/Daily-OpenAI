# UNIT-DECROUND-001 — Đối soát học bổng: sai lệch tiền do quy tắc làm tròn

## Mục tiêu
Tái hiện sai lệch khi tính khoản hỗ trợ theo đơn vị tiền tệ, thu thập bằng chứng ở các giá trị biên và xác nhận quy tắc nghiệp vụ bằng regression tests.

## Bối cảnh thực tế
Trường đại học gửi lệnh chi học bổng theo từng sinh viên. Báo cáo từ hệ thống .NET đôi khi lệch 0,01 so với bảng đối soát của đơn vị thanh toán, dù tổng số dòng và dữ liệu đầu vào giống nhau.

## Bạn cần làm gì
Chạy bản starter; lập bảng input/output, đề xuất ít nhất hai giả thuyết; xác định quy tắc tính tiền đúng theo hợp đồng trong `docs/scenario.md`; sửa `starter/Program.cs` rồi chạy verify.

## Yêu cầu môi trường
.NET SDK 8.x; PowerShell 7 hoặc Windows PowerShell; không cần mạng, database hay NuGet package ngoài SDK.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` chạy bản starter chưa chỉnh sửa và xác nhận có ít nhất một dòng không khớp hợp đồng. Lưu output để so sánh.

## Những gì cần quan sát
Đối chiếu từng khoản và tổng tiền; chú ý những giá trị có phần lẻ nằm đúng ranh giới. Đừng suy luận từ tổng tiền mà bỏ qua từng khoản.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Chạy `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và phân tích](solution/README.md)

## Expected Results
Trước: có khoản thanh toán lệch 0,01 và `verify.ps1` thất bại. Sau: toàn bộ khoản và tổng khớp quy tắc hợp đồng; `verify.ps1` thành công.

## Estimated Time
40 phút.

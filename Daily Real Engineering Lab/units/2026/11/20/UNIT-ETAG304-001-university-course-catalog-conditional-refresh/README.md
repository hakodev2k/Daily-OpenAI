# UNIT-ETAG304-001 — Bản cập nhật catalog khóa học biến thành lỗi đồng bộ

## Mục tiêu
Điều tra một luồng HTTP đồng bộ catalog bằng evidence; phân biệt phản hồi không có body với lỗi downstream, bảo toàn dữ liệu đã xác nhận và kiểm thử các nhánh response.

## Bối cảnh thực tế
Một trường đại học đồng bộ danh sách khóa học từ đối tác qua API. Lần đầu đồng bộ thành công. Những lần làm mới tiếp theo đôi khi trả về thông báo lỗi dù API đối tác vẫn hoạt động và dữ liệu chưa thay đổi.

## Bạn cần làm gì
1. Chạy kịch bản hai lần refresh và ghi lại HTTP status, request headers, số lần tải payload và trạng thái cache.
2. Đưa ra ít nhất hai giả thuyết trước khi sửa code.
3. Sửa **starter/Program.cs**, không sửa fixture để che triệu chứng.
4. Chạy verify, rồi so sánh reference solution.

## Yêu cầu môi trường
.NET SDK 8.0. Không cần tài khoản cloud, NuGet package ngoài hay network.

## Chạy nhanh
Tại thư mục unit:
```powershell
./reproduce.ps1
./verify.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` chạy starter trong chế độ `repro`. Quan sát refresh đầu và refresh kế tiếp.

## Những gì cần quan sát
Ghi status, ETag, số payload được nhận, số item sau mỗi lần làm mới, và exit code. Không suy luận chỉ từ một exception.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-1.md) · [Hint 2](hints/hint-2.md) · [Hint 3](hints/hint-3.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi tự sửa.

## Expected Results
[Xem tiêu chí](expected-results/README.md).

## Estimated Time
45–60 phút.

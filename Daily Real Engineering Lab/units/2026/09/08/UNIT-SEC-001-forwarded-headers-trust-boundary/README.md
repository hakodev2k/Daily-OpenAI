# UNIT-SEC-001 — Internal-only endpoint đôi khi mở cho request từ Internet

## Mục tiêu
Điều tra một lỗi trust-boundary trong ASP.NET Core khi ứng dụng quyết định quyền truy cập dựa trên địa chỉ client sau reverse proxy.

## Bối cảnh thực tế
Một API chạy sau reverse proxy có endpoint `/ops/cache/clear` chỉ dành cho mạng nội bộ. Team dùng địa chỉ client mà application nhìn thấy để quyết định request có phải internal hay không. Sau một thay đổi hạ tầng, security review phát hiện request từ Internet có thể được application phân loại thành internal trong một số trường hợp.

## Bạn cần làm gì
1. Chạy starter và xác nhận request bình thường hoạt động như mong đợi.
2. Chạy script reproduce để tái hiện request không đáng tin cậy được phân loại sai.
3. Ghi ít nhất 2 hypotheses trước khi xem hints.
4. Xác định dữ liệu nào đang được coi là authoritative và ai có quyền tạo dữ liệu đó.
5. Sửa `starter/` để request Internet không thể tự biến thành internal chỉ bằng metadata do client gửi.
6. Giữ khả năng nhận diện request nội bộ khi metadata đến từ boundary đáng tin cậy.
7. Chạy `verify.ps1`.
8. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 5.1+ hoặc PowerShell 7+

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Kết quả phân loại của request không có forwarding metadata.
- Kết quả phân loại khi request tự cung cấp forwarding metadata.
- Application có phân biệt được metadata do trusted infrastructure thêm với metadata do Internet client tự gửi hay không.
- Business impact: endpoint vận hành có thể được mở ngoài trust boundary dự kiến.

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
> Spoiler: chỉ xem sau khi đã tự thử.
- [Reference Solution](solution/README.md)

## Expected Results
**Before:** một request Internet có thể khiến application tự phân loại nó là internal bằng metadata đầu vào.

**After:** chỉ metadata đã đi qua trust boundary được cấu hình mới có thể ảnh hưởng đến client identity; request trực tiếp không được nâng quyền phân loại.

Nếu không reproduce được, kiểm tra port `5074` có đang bị process khác sử dụng hay không rồi chạy lại.

## Estimated Time
35–50 phút.

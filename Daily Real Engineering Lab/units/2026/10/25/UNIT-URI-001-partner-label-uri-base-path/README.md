# UNIT-URI-001 — Một API đối tác trả 404 dù endpoint khác vẫn hoạt động

## Mục tiêu
Tái hiện sự cố định tuyến outbound HTTP, thu thập request evidence và sửa hợp đồng ghép endpoint để nhiều môi trường triển khai hoạt động nhất quán.

## Bối cảnh thực tế
Hệ thống kho gửi yêu cầu tạo nhãn vận chuyển đến hãng vận chuyển. Sau khi đối tác chuyển gateway sang đường dẫn API có prefix, thao tác tra cứu trạng thái vẫn trả 200 nhưng tạo nhãn trả 404. Host, chứng thực và dữ liệu gửi đi không đổi. Fake partner server chạy in-memory, không gọi Internet.

## Bạn cần làm gì
1. Chạy starter, ghi lại status và URI path thực tế của cả hai request.
2. Đề xuất ít nhất hai giả thuyết; kiểm chứng bằng evidence thay vì đoán từ 404.
3. Chỉ chỉnh sửa `starter/LabelGateway.cs`, giữ nguyên contract, không hardcode hostname.
4. Chạy verify với hai API prefix khác nhau, sau đó mới xem reference solution.

## Yêu cầu môi trường
.NET SDK 8 (global.json có feature-band roll-forward), PowerShell 7 hoặc Windows PowerShell. Không cần Docker, Azure, Internet hay NuGet bên ngoài SDK.

## Chạy nhanh
Từ thư mục unit:
```powershell
./setup.ps1
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script trả exit code 0 **chỉ khi triệu chứng của starter gốc được tái hiện**; đây không phải phép thử bản sửa. Sau khi sửa, dùng verify.

## Những gì cần quan sát
- Request nào thất bại và request nào thành công?
- URI path fake partner nhận có khớp route contract không?
- Khi thay API prefix theo môi trường, hiện tượng có thay đổi không?
- Phân biệt 404 do routing với 404 do resource không tồn tại như thế nào?
- Nếu không reproduce: kiểm tra SDK và chạy lại starter chưa chỉnh sửa.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md).

## Reference Solution
⚠️ Spoiler — chỉ mở sau khi tự sửa: [Reference Solution](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). `./verify.ps1` chạy **starter đã chỉnh sửa**, không chạy solution.

## Estimated Time
35–45 phút. Ghi suy luận tại [workspace/my-investigation.md](workspace/my-investigation.md).

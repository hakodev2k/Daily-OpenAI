# UNIT-OCACHE-001 — Thông báo đóng hồ bơi hiển thị sai ngôn ngữ sau đợt tăng tải

## Mục tiêu
Điều tra một sự cố response không nhất quán giữa các khách truy cập cùng một endpoint ASP.NET Core. Dùng evidence và phép thử đối chứng để xác định ranh giới cần thay đổi, rồi chứng minh cả nội dung lẫn hành vi phục vụ lặp lại đều đúng.

## Bối cảnh thực tế
Cổng thông tin hồ bơi thành phố phát thông báo đóng cửa đột xuất bằng tiếng Việt và tiếng Anh. Sau khi triển khai tối ưu hiệu năng, tổng số request vẫn trả HTTP 200, latency giảm nhưng một số người dùng tiếng Anh nhận nội dung tiếng Việt. Vấn đề phụ thuộc thứ tự truy cập, không phụ thuộc tài khoản.

## Bạn cần làm gì
1. Reproduce bằng hai request có cùng URL nhưng khác ngôn ngữ, ghi lại `language`, `message` và `renderCount`.
2. Nêu ít nhất ba giả thuyết: dữ liệu nguồn, nhận diện ngôn ngữ, tầng phục vụ response; xác định phép thử phân biệt.
3. Chỉ sửa `starter/Program.cs`; giữ nguyên endpoint, JSON schema, `renderCount` và thời hạn tối ưu đã được cấu hình.
4. Chạy `verify.ps1`, kiểm tra cả hai thứ tự truy cập, lặp lại trong cùng ngôn ngữ và hai biến thể ngôn ngữ.
5. Viết trade-off giữa tính đúng đắn, cache hit ratio và số lượng cache entries.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell 5.1. Không cần Docker, cloud, database hoặc NuGet bên ngoài shared framework.

## Chạy nhanh
Tại thư mục unit:
```powershell
./setup.ps1
./reproduce.ps1
./verify.ps1
```
Nếu dùng Windows PowerShell có execution policy chặn script, chạy `powershell -ExecutionPolicy Bypass -File .\reproduce.ps1`.

## Cách reproduce vấn đề
`reproduce.ps1` tự build, khởi động server local trên cổng ngẫu nhiên, đợi `/health`, gửi chuỗi request theo thứ tự xác định và dừng server. Exit code 0 nghĩa là **đã quan sát được triệu chứng ban đầu**, không phải code đúng. Sau khi sửa đúng, reproduction sẽ không còn thành công.

Dùng `./run.ps1` để chạy thủ công, sau đó gửi request với header `Accept-Language: vi-VN` hoặc `en-US` tới `http://127.0.0.1:5193/api/pool/closure`.

## Những gì cần quan sát
- Cùng URL nhưng khác `Accept-Language` có cho ra hai ngôn ngữ tương ứng không?
- Response đầu tiên có ảnh hưởng response tiếp theo không?
- `renderCount` có giữ nguyên khi lặp lại một biến thể trong cùng phiên server không?
- Health endpoint, status code và schema có thay đổi không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và evidence.
3. Tự sửa trong `starter/`.
4. Verify cả correctness và cache reuse.
5. Chỉ sau đó xem reference solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)
- `reproduce.ps1` exit 0 khi symptom gốc xuất hiện.
- `verify.ps1` exit 0 chỉ khi **starter đã chỉnh sửa** đáp ứng cả nội dung và khả năng reuse.

## Estimated Time
50–65 phút (L3 Production Incident, D4).

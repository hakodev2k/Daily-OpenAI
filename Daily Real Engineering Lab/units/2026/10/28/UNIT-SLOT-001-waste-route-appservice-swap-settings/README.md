# UNIT-SLOT-001 — Production đọc nhầm dữ liệu thử nghiệm sau App Service slot swap

## Mục tiêu
Điều tra một sự cố cấu hình khi triển khai Azure App Service bằng deployment slots; phân loại cấu hình gắn với **môi trường** và cấu hình gắn với **release**, tái hiện và kiểm chứng rollback bằng fixture local.

## Bối cảnh thực tế
Hệ thống tối ưu tuyến thu gom rác đô thị có hai slot: `production` phục vụ điều phối viên và `staging` để thử thuật toán mới. Sau một lần swap đã được phê duyệt, production báo tuyến đường không khớp dữ liệu thật; bản build mới vẫn xuất hiện đúng. Nhóm vận hành cần xác định điều gì đã thay đổi và thiết kế kiểm tra trước lần phát hành kế tiếp.

## Bạn cần làm gì
1. Chạy fixture và ghi lại bảng giá trị cấu hình của cả hai slot trước/sau swap.
2. Nêu ít nhất hai giả thuyết, xác định ranh giới giữa release config và environment config.
3. Chỉ chỉnh sửa `starter/fixtures/slots.json`; không đổi tên key, giá trị endpoint hoặc logic mô phỏng.
4. Chạy `verify.ps1` để kiểm tra cả swap lẫn rollback; giải thích thêm bước xác minh cần có trên Azure thực.

## Yêu cầu môi trường
- .NET SDK 8.x, PowerShell 5.1+ hoặc PowerShell 7.
- Không cần Azure subscription, Docker, database, internet hay NuGet ngoài SDK. Đây là **mô phỏng giới hạn ở app settings**; không mô phỏng warm-up, routing, connection draining hoặc toàn bộ quy trình swap của Azure.

## Chạy nhanh
Từ thư mục unit:

```powershell
.\reproduce.ps1
```

Hoặc:

```bash
dotnet run --project starter/SlotSwapLab.csproj --configuration Release -- --reproduce
```

## Cách reproduce vấn đề
`reproduce.ps1` dùng fixture baseline cố định để chứng minh triệu chứng độc lập với file bạn sẽ chỉnh sửa. Quan sát hai slot và các giá trị sau swap. Exit code 0 ở bước này nghĩa là **đã tái hiện lỗi**, không có nghĩa là đã sửa.

## Những gì cần quan sát
- Endpoint mà `production` sử dụng sau swap so với trước đó.
- Release label và feature flag nào di chuyển sang `production`.
- Nhãn môi trường của hai slot có còn đúng hay không.
- Nếu swap lại để rollback, các giá trị có trở về trạng thái ban đầu không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và bằng chứng vào [workspace](workspace/my-investigation.md).
3. Thử thay đổi cấu hình được phép.
4. Verify bằng `.\verify.ps1`; exit code phải bằng 0.
5. Chỉ sau đó mới xem reference solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Expected Results
- **Before:** `REPRODUCED`; production sau swap có endpoint dữ liệu khác ban đầu dù release mới đã đến đúng slot.
- **After:** `VERIFIED`; production/staging giữ đúng endpoint môi trường, release config vẫn được hoán đổi, rollback phục hồi toàn bộ giá trị.

## Reference Solution
**Spoiler:** [Cấu hình tham chiếu và phân tích](solution/README.md). Không xem trước khi tự điều tra.

## Estimated Time
45–60 phút. Đây là L4 / D4; trọng tâm là điều tra, sửa cấu hình và đặt deployment gate có thể kiểm chứng.

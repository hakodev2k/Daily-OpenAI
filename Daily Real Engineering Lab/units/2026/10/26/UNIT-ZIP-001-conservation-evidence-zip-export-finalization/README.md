# UNIT-ZIP-001 — Gói ZIP hồ sơ kiểm định tải về nhưng không mở được

## Mục tiêu
Điều tra một lỗi export file trong .NET: mã nghiệp vụ chạy không ném exception, dữ liệu đầu vào hợp lệ, nhưng người nhận không mở được file ZIP. Tập trung vào thu thập bằng chứng, xác định ranh giới tạo dữ liệu, sửa và kiểm tra regression.

## Bối cảnh thực tế
Hệ thống bảo tồn di sản cung cấp chức năng tải gói hồ sơ kiểm định gồm `manifest.json` và báo cáo hiện trường. Dashboard báo export thành công; ứng dụng lưu trữ bên ngoài lại báo file ZIP không hợp lệ. Nhóm vận hành xác nhận nội dung từng tài liệu có sẵn và không có lỗi I/O rõ ràng.

## Bạn cần làm gì
1. Chạy bản starter và ghi lại kết quả với cả gói có tài liệu và gói rỗng.
2. Nêu ít nhất hai giả thuyết có thể giải thích lỗi; phân biệt lỗi nội dung entry với lỗi cấu trúc container.
3. Chỉ sửa `starter/` để ZIP đọc được bằng thư viện tiêu chuẩn, giữ nguyên tên và nội dung entry.
4. Chạy `verify.ps1` và ghi bằng chứng trước/sau; so sánh với reference solution sau khi tự thử.

## Yêu cầu môi trường
- .NET 8 SDK, PowerShell 7+ (hoặc Windows PowerShell 5.1).
- Không cần Docker, Azure, database hoặc NuGet package bên ngoài.

## Chạy nhanh
Tại thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```
Trên Windows bị chặn script policy, dùng `pwsh -NoProfile -File ./scripts/reproduce.ps1` hoặc `powershell -NoProfile -File ./scripts/reproduce.ps1` theo policy máy; không cần tắt bảo vệ toàn hệ thống.

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` chạy bản starter nguyên gốc, xác nhận tình trạng ZIP không hợp lệ bằng bộ đọc độc lập trong cùng chương trình. Exit code 0 ở bước này nghĩa là **đã tái hiện lỗi**, không có nghĩa là đã sửa. Sau khi sửa, dùng `./scripts/verify.ps1` để kiểm tra chính `starter/` đã chỉnh sửa.

## Những gì cần quan sát
- Kích thước byte export và trạng thái đọc ZIP qua 3 lần tạo.
- Tình trạng gói không có entry.
- Lỗi xuất hiện ở lúc tạo byte hay lúc consumer mở container?
- Có thay đổi nào ở nội dung entry, số entry hay chỉ ở tính hợp lệ của container?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify cả gói thông thường và gói rỗng.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [solution/README.md](solution/README.md) và [reference code](solution/Program.cs). Chỉ mở sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
50 phút. Nếu không reproduce: kiểm tra `dotnet --version`, `dotnet restore`, chạy lại bản starter chưa sửa, và ghi nguyên văn exception trong `workspace/`.

# UNIT-RECORD-002 — Bản chụp manifest hành lý thay đổi sau bàn giao

## Mục tiêu
Điều tra tính nhất quán của bản chụp dữ liệu tại thời điểm bàn giao; bảo vệ dữ liệu audit khỏi các thay đổi về sau và kiểm chứng bằng regression test.

## Bối cảnh thực tế
Một hãng vận chuyển hàng không dùng dịch vụ .NET để ghi nhận bag tags cho từng chuyến bay. Sau mỗi lượt kiểm tra, hệ thống chụp manifest gửi bộ phận đối soát. Sáng hôm sau, báo cáo đã bàn giao hiển thị số kiện lớn hơn số kiện thực tế tại thời điểm chụp, mặc dù timestamp vẫn đúng. Một nhóm kiểm thử còn chứng minh rằng dữ liệu đang phục vụ có thể bị thay đổi thông qua đối tượng báo cáo trả về.

## Bạn cần làm gì
1. Chạy bản gốc và lưu bằng chứng về hai lần chụp cùng một chuyến.
2. Nêu ít nhất hai hypothesis và kiểm tra chúng trên code hoặc kết quả chạy.
3. Sửa **starter/** sao cho manifest đã chụp không bị thay đổi bởi scan sau đó hoặc bởi người đọc bản chụp.
4. Chạy verify và xác nhận chức năng thêm bag tag, tách biệt chuyến bay và timestamp không bị regression.
5. Chỉ sau khi tự sửa, mở reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x (cấu hình trong `global.json`, roll-forward trong cùng major).
- PowerShell 7 hoặc Windows PowerShell 5.1; không cần database, Docker, cloud hay NuGet package bên ngoài.

## Chạy nhanh
Tại thư mục lab:
```powershell
./run.ps1
./reproduce.ps1
# Sửa starter/ManifestService.cs rồi chạy:
./verify.ps1
```
Nếu execution policy chặn script, chạy `pwsh -NoProfile -File ./reproduce.ps1` (hoặc `powershell -NoProfile -File ...` trên Windows với policy cho phép).

## Cách reproduce vấn đề
`reproduce.ps1` chạy code **starter nguyên bản** và trả exit code 0 khi tái hiện đúng hai triệu chứng đã định nghĩa; nếu đã sửa starter, bước này có thể không còn tái hiện. `verify.ps1` chạy chính code starter mà bạn vừa chỉnh sửa và trả lỗi nếu contract chưa đạt.

## Những gì cần quan sát
- Bag count của snapshot đầu tiên ngay sau scan tiếp theo.
- Snapshot mới nhất có chứa bag tag không được quét chính thức hay không.
- Timestamp chụp và dữ liệu chuyến bay khác có bị ảnh hưởng không.
- Những đối tượng nào có quyền sửa dữ liệu sau khi snapshot được tạo.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix tại starter.
4. Verify bằng script độc lập.
5. Sau đó mới xem [Reference Solution](solution/README.md).

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Chỉ mở sau khi tự điều tra và sửa](solution/README.md).

## Expected Results
[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md)

## Estimated Time
35–45 phút. Có thể mở rộng bằng [Senior Challenge](extensions/senior-challenge.md).

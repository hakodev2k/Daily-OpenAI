# UNIT-ENUMJSON-001 — Shelter occupancy status drift

## Mục tiêu
Điều tra lỗi hợp đồng dữ liệu khi ingest JSON trạng thái nơi trú ẩn, thu thập bằng chứng và sửa an toàn.

## Bối cảnh thực tế
Đối tác nâng cấp feed; API vẫn trả thành công nhưng một shelter không được đưa vào quy trình review. Dữ liệu cần được xử lý an toàn trong trường hợp schema drift.

## Bạn cần làm gì
Chạy reproduce, ghi hai giả thuyết, sửa starter/Program.cs, bổ sung bằng chứng và chạy verify.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell; không cần cloud hay database.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` kiểm tra starter ban đầu có triệu chứng; exit 0 nghĩa là đã tái hiện.

## Những gì cần quan sát
Ghi lại giá trị nhận từ feed, kết quả routing và số bản ghi được kiểm tra. Đối chiếu với contract mong muốn.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng `./scripts/verify.ps1` trên starter đã sửa.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
[Spoiler](solution/README.md)

## Expected Results
Reproduce phát hiện record bị bỏ qua; verify yêu cầu input sai bị từ chối và input hợp lệ vẫn được xử lý.

## Estimated Time
40 phút.

# UNIT-OFFSYNC-001 — Đồng bộ Work Order offline khi hai thiết bị cùng chỉnh sửa

## Mục tiêu

Điều tra một sự cố đồng bộ Work Order khi kỹ thuật viên làm việc offline; tái hiện bằng chương trình .NET 8, thu thập bằng chứng, sửa hành vi đồng bộ và bảo vệ các bất biến bằng kiểm thử. Sau đó viết một Architecture Decision Record (ADR) ngắn cho phương án triển khai thực tế.

## Bối cảnh thực tế

Một đội bảo trì hạ tầng cấp nước sử dụng tablet để cập nhật trạng thái công việc và ghi chú tại hiện trường. Hai thiết bị có thể mất mạng nhiều giờ. Khi kết nối trở lại, một số thay đổi hợp lệ biến mất, còn thay đổi cũ đôi khi ghi đè thông tin mới. Bộ phận vận hành yêu cầu không làm mất dữ liệu và phải phân biệt rõ thay đổi có thể tự hợp nhất với thay đổi cần người xử lý.

## Bạn cần làm gì

1. Chạy starter, ghi nhận thứ tự các thay đổi và trạng thái cuối cùng.
2. Đưa ra ít nhất hai giả thuyết, chỉ rõ bằng chứng phân biệt chúng.
3. Sửa **starter/SyncEngine.cs** để đạt các yêu cầu trong [scenario](docs/scenario.md), không chỉnh assertions trong Program.cs.
4. Chạy verification; viết ADR ngắn tại [workspace](workspace/my-investigation.md), so sánh ít nhất hai chính sách conflict.
5. Chỉ sau đó đối chiếu reference solution.

## Yêu cầu môi trường

- .NET 8 SDK (có thể dùng SDK 8.0.x tương thích theo global.json).
- PowerShell 7 hoặc Windows PowerShell; không cần database, Docker, Azure hay NuGet ngoài .NET SDK.

## Chạy nhanh

Từ thư mục unit:

```powershell
./setup.ps1
./reproduce.ps1
./verify.ps1
```

`reproduce.ps1` thành công khi **phát hiện đúng lỗi của starter**; `verify.ps1` ban đầu phải thất bại và chỉ thành công sau khi sửa code learner-editable.

## Cách reproduce vấn đề

Chạy `./reproduce.ps1`. So sánh bảng kết quả của 5 lần gửi thay đổi với [timeline](evidence/timeline.md). Ghi lại kết quả của thiết bị B, lần gửi lại của B và thay đổi cuối của A.

## Những gì cần quan sát

- Một thay đổi trên trường độc lập không xuất hiện trong bản ghi cuối.
- Một thay đổi trên trường đã bị sửa bởi thiết bị khác lại được chấp nhận.
- Thứ tự nhận sự kiện và đồng hồ của thiết bị có thể khác nhau.
- Retry một sự kiện có thể tạo kết quả không phù hợp với yêu cầu idempotency.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Mở lời giải và code tham chiếu](solution/README.md) sau khi đã tự điều tra.

## Expected Results

[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time

70–90 phút · L5 Design Decision / D4 Production Troubleshooting.

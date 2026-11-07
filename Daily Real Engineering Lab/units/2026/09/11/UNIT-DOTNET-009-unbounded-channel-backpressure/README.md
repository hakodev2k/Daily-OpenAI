# UNIT-DOTNET-009 — Unbounded Channel Backpressure

## Mục tiêu

Điều tra một pipeline xử lý nền có throughput đầu vào cao hơn khả năng xử lý của consumer, xác định vì sao backlog tăng mạnh dù không có exception, rồi sửa trực tiếp starter để hệ thống áp dụng backpressure mà vẫn xử lý đủ dữ liệu.

## Bối cảnh thực tế

Một service nhận các work item từ upstream và đẩy chúng vào pipeline nội bộ. Trong giờ cao điểm, producer nhận dữ liệu rất nhanh còn consumer xử lý chậm hơn. CPU không cao, request đầu vào vẫn thành công, nhưng backlog nội bộ tăng nhanh và memory pressure xuất hiện sau một thời gian chạy.

Lab dùng một mô phỏng deterministic nhỏ để bạn quan sát cơ chế này ngay trên local.

## Bạn cần làm gì

1. Chạy `reproduce.ps1` và ghi lại `maxBacklog`.
2. Đọc starter code và hình thành ít nhất 2 hypotheses.
3. Sửa code trong `starter/` để giới hạn backlog và tạo backpressure đúng chỗ.
4. Không được drop work item; toàn bộ 200 item vẫn phải được consume.
5. Chạy `verify.ps1` để kiểm tra cả correctness và backlog bound.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần Docker, database hay dịch vụ ngoài

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script chạy starter và chỉ pass khi symptom gốc xuất hiện rõ: xử lý đủ work item nhưng backlog vượt xa ngưỡng vận hành mong muốn.

## Những gì cần quan sát

- `produced` và `consumed` cuối cùng vẫn bằng nhau.
- `maxBacklog` tăng cao khi producer chạy nhanh hơn consumer.
- Không cần exception để hệ thống rơi vào trạng thái vận hành xấu.
- Thời gian producer hoàn thành và thời gian consumer hoàn thành phản ánh hai tốc độ khác nhau.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before:
- `produced=200`
- `consumed=200`
- `maxBacklog` lớn hơn 8 rõ rệt

After:
- `produced=200`
- `consumed=200`
- `maxBacklog` không vượt quá 8
- không drop item và không chuyển lỗi sang consumer

Nếu symptom không reproduce, hãy chạy lại sau khi chắc chắn starter chưa bị sửa và đang dùng .NET 8 SDK.

## Estimated Time

35–50 phút.

# UNIT-CHAN-002 — Nhật ký kiểm định lô dược phẩm thiếu sự kiện

## Mục tiêu

Điều tra pipeline audit nội bộ chạy trên .NET khi hệ thống nhận burst sự kiện; dùng bằng chứng để phân biệt **API báo nhận thành công** với **sự kiện thực sự đến được consumer**, rồi bảo vệ tính đầy đủ, thứ tự và hành vi khi hủy tác vụ.

## Bối cảnh thực tế

Một dịch vụ kiểm định lô dược phẩm ghi audit event cho từng bước phê duyệt. Sau khi tối ưu bộ đệm in-memory để giới hạn tài nguyên, bộ phận compliance phát hiện vài mã bước không xuất hiện trong bản export, dù publisher không báo lỗi. Bản mô phỏng dùng dữ liệu synthetic, không có thông tin bệnh nhân.

## Bạn cần làm gì

1. Chạy starter, ghi lại số event đã gửi, event được consumer nhận và trạng thái producer trước khi consumer bắt đầu.
2. Nêu ít nhất hai hypothesis, xác định nơi sự kiện có thể biến mất.
3. Thay đổi **starter/AuditPipeline.cs** để đạt yêu cầu: không mất event, giữ thứ tự, có cơ chế điều tiết khi consumer chưa kịp xử lý, hủy tác vụ chờ đúng cách.
4. Chạy verify trên chính starter đã sửa; đối chiếu reference sau khi tự hoàn thành.

## Yêu cầu môi trường

- .NET SDK 10.0.100 trở lên trong feature band 10.0 (xem global.json); không cần NuGet ngoài SDK.
- PowerShell 7+ hoặc Windows PowerShell 5.1; không cần Docker, Azure hay database.

## Chạy nhanh

Từ thư mục unit:

\`\`\`powershell
./setup.ps1
./run.ps1
./reproduce.ps1
\`\`\`

Trên macOS/Linux có thể dùng \`dotnet run --project starter/AuditLab.Starter.csproj -- demo\`, thay \`demo\` bằng \`reproduce\` hoặc \`verify\`.

## Cách reproduce vấn đề

Chạy \`./reproduce.ps1\` với starter nguyên bản. Script **trả exit code 0 khi đã xác nhận triệu chứng lỗi**, không có nghĩa ứng dụng đạt yêu cầu. Chạy \`./verify.ps1\` trước khi sửa để thấy contract chưa đạt (exit code khác 0).

## Những gì cần quan sát

- Publisher cố gửi 5 event có sequence 1..5, nhưng consumer nhận bao nhiêu?
- Producer đã hoàn thành trước khi consumer bắt đầu chưa?
- Một publisher đang chờ tài nguyên có phản ứng với CancellationToken không?
- Có exception nào không, và điều đó có chứng minh delivery hay không?

Ghi số liệu và hypothesis vào [workspace/my-investigation.md](workspace/my-investigation.md).

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trên starter.
4. Verify cả tính đúng đắn và tính chất engineering.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [solution/README.md](solution/README.md) và [reference code](solution/AuditPipeline.cs). Chỉ xem sau khi tự điều tra.

## Expected Results

- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time

60–75 phút (L3 / D4).

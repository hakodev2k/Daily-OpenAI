# UNIT-API-002 — Hai lượt chỉnh sửa bảng kiểm công viên cùng được chấp nhận

## Mục tiêu

Điều tra một lỗi cập nhật đồng thời trong API có điều kiện, sử dụng ETag/If-Match, rồi chứng minh tính đúng đắn bằng kiểm thử race có thể lặp lại.

## Bối cảnh thực tế

Hệ thống quản lý bảo trì công viên cho phép nhân viên sửa ghi chú kiểm tra thiết bị. Hai nhân viên cùng mở bản ghi revision 7 và gửi thay đổi gần như đồng thời. Cả hai nhận kết quả thành công, nhưng hệ thống chỉ lưu một ghi chú; lịch sử revision không phản ánh hai lượt ghi được chấp nhận. Điều này gây mất thông tin khi bàn giao ca.

Bài tập dùng một local .NET console harness mô phỏng ranh giới HTTP (status code, If-Match, ETag) và tầng lưu trữ, **không** khởi chạy HTTP server hoặc database thật. Có điểm đồng bộ chỉ phục vụ tái hiện race một cách xác định.

## Bạn cần làm gì

1. Tái hiện hai yêu cầu cập nhật cùng ETag.
2. Thu thập status code, ETag và trạng thái bản ghi sau hai yêu cầu.
3. Viết ít nhất hai hypothesis về vị trí vi phạm hợp đồng cập nhật.
4. Sửa `starter/RevisionedChecklistStore.cs` để mỗi revision chỉ chấp nhận tối đa một bản cập nhật hợp lệ.
5. Giữ nguyên xử lý thiếu, sai định dạng và stale If-Match; xác minh không có regression.

## Yêu cầu môi trường

- .NET SDK 8.x; `global.json` cho phép các feature band mới hơn.
- PowerShell 7 (hoặc Windows PowerShell hỗ trợ script); không cần NuGet package ngoài SDK.
- Không cần Docker, cloud, API key hoặc kết nối mạng khi SDK đã sẵn có.

## Chạy nhanh

Tại thư mục unit:

```powershell
pwsh -File ./setup.ps1
pwsh -File ./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy **starter chưa sửa**, chủ động đặt hai yêu cầu vào cùng thời điểm và chỉ trả exit code 0 khi tái hiện được triệu chứng hai cập nhật đều thành công. Sau khi tự sửa code trong `starter/`, chạy `pwsh -File ./verify.ps1`. Không copy reference solution trước khi tự thử.

Nếu chỉ có shell Bash: `dotnet run --project starter/ChecklistLab.csproj -- reproduce` và sau khi sửa dùng `-- verify`.

## Những gì cần quan sát

- Hai mã phản hồi khi cả hai client gửi `If-Match: "7"`.
- Revision cuối cùng và ghi chú còn lại.
- Phản hồi khi thiếu If-Match, khi tag không hợp lệ và khi client dùng ETag cũ.
- Tại sao một cặp status code có thể vi phạm kỳ vọng dù revision cuối trông bình thường.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ mở [Reference Solution](solution/README.md) sau khi tự điều tra và thử sửa. Có source code tham khảo riêng trong `solution/`.

## Expected Results

[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md). Ghi lại phân tích trong [workspace](workspace/my-investigation.md).

## Estimated Time

50 phút. Tài liệu chuẩn: [references](docs/references.md).

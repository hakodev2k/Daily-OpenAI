# UNIT-AZURE-003 — Concurrent Blob Edit Loses an Update

## Mục tiêu

Điều tra một lỗi concurrency ở document storage nơi hai người dùng cùng chỉnh sửa một blob từ cùng phiên bản ban đầu. Học cách dùng evidence để phân biệt lỗi application workflow với lỗi storage write contract, sau đó bảo vệ dữ liệu mà không serialize toàn bộ hệ thống.

## Bối cảnh thực tế

Một CMS nội bộ lưu nội dung bài viết vào blob storage. Hai editor mở cùng một bài gần như cùng lúc. Cả hai chỉnh sửa khác nhau và bấm Save cách nhau rất ngắn. UI của cả hai đều báo thành công, nhưng khi reload chỉ còn nội dung của lần save sau.

Production telemetry không có exception và storage availability bình thường. Team cần xác định tại sao một update hợp lệ biến mất và đưa ra fix có thể áp dụng cho Azure Blob Storage.

## Bạn cần làm gì

1. Chạy `./reproduce.ps1` để tạo symptom ban đầu.
2. Ghi evidence và ít nhất hai hypothesis vào `workspace/my-investigation.md`.
3. Đọc code trong `starter/`, đặc biệt boundary giữa document service và blob store.
4. Sửa code learner-editable trong `starter/`.
5. Chạy `./verify.ps1` cho đến khi verification pass.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell
- Không cần Azure subscription, Docker hay Azurite. Lab dùng local simulator để mô phỏng write contract cần điều tra.

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/11/UNIT-AZURE-003-concurrent-blob-edit-lost-update"
./reproduce.ps1
```

## Cách reproduce vấn đề

Script tạo một document ban đầu, cho hai editor đọc cùng state, sau đó save hai thay đổi theo thứ tự xác định. Reproduction PASS khi symptom production được chứng minh: cả hai save path đều hoàn tất nhưng state cuối không chứa thay đổi của editor trước.

## Những gì cần quan sát

- Metadata trả về khi mỗi editor đọc document.
- Metadata của blob thay đổi sau mỗi successful write.
- Cả hai editor bắt đầu từ cùng một state.
- Không có timeout, retry hay exception trong starter flow.
- Final content phản ánh chỉ một trong hai edit.

Tập trung vào evidence ở read/write boundary, không giả định ngay rằng cần distributed lock.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi đã reproduce và tự thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Trước khi sửa

- Hai editor đọc cùng version của document.
- Save A hoàn tất.
- Save B cũng hoàn tất dù state đã thay đổi từ lúc B đọc.
- Final content là edit B; edit A không còn tồn tại.

### Sau khi sửa

- Save A hoàn tất.
- Save B từ snapshot cũ bị từ chối rõ ràng thay vì ghi đè im lặng.
- Final content vẫn là edit A.
- `verify.ps1` exit code `0`.

Nếu không chạy được, kiểm tra `dotnet --info` và bảo đảm .NET 8 SDK khả dụng.

## Estimated Time

35–55 phút.

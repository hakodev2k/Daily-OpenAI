# UNIT-DEPLOY-003 — Windows chạy được nhưng Linux không tìm thấy template

Một service render invoice chạy ổn trên máy developer Windows. Sau khi publish sang Linux container, request render invoice bắt đầu trả lỗi `FileNotFoundException` cho template HTML. Team đã kiểm tra artifact và xác nhận template thực sự có trong package deploy.

## Mục tiêu

Điều tra vì sao cùng một cấu hình path có vẻ hợp lệ ở một môi trường nhưng thất bại ở môi trường khác, xác định contract path đúng và sửa mà không dựa vào behavior riêng của Windows.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/13/UNIT-DEPLOY-003-linux-path-case-sensitivity"
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy starter với một filesystem simulator có semantics case-sensitive tương tự Linux. Script chỉ PASS khi starter tái hiện được lỗi lookup dù file tồn tại trong artifact.

## Những gì cần quan sát

- Artifact manifest có chứa template invoice.
- Runtime request dùng một path được lấy từ cấu hình.
- Lookup trả `FileNotFoundException`.
- Không thay đổi working directory, không thiếu file và không có race condition.

Ghi ít nhất 3 hypotheses vào `workspace/my-investigation.md` trước khi sửa.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi evidence và hypotheses.
3. Sửa code/config trong `starter/`.
4. Chạy `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem `solution/README.md`.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi đã tự reproduce và thử fix.

- [Reference solution](solution/README.md)

## Expected Results

Before:
- starter exit code khác `0`;
- log cho thấy template tồn tại trong artifact manifest nhưng lookup thất bại.

After:
- `verify.ps1` exit code `0`;
- lookup trả đúng nội dung template;
- fix không dựa vào việc chuyển filesystem sang case-insensitive.

## Estimated Time

Khoảng 40 phút.

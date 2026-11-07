# UNIT-AZURE-002 — Blob ETag Lost Update

## Mục tiêu

Điều tra một race condition khi hai worker cùng cập nhật một blob cấu hình và cả hai đều báo thành công, nhưng thay đổi của một worker biến mất.

## Bối cảnh thực tế

Một CMS lưu metadata xuất bản dưới dạng blob JSON. Hai background worker có thể đọc cùng phiên bản gần như đồng thời rồi cập nhật các trường khác nhau. Sau một đợt publish song song, audit log cho thấy cả hai lệnh đều thành công nhưng blob cuối cùng chỉ chứa thay đổi của worker chạy sau.

## Bạn cần làm gì

1. Reproduce hiện tượng mất cập nhật.
2. Ghi lại evidence và ít nhất hai hypothesis.
3. Xác định contract còn thiếu giữa bước read và write.
4. Sửa `starter/` để một stale writer không thể âm thầm ghi đè phiên bản mới hơn.
5. Chạy `verify.ps1`.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell
- Không cần Azure subscription; lab dùng local storage simulator deterministic.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Hai worker đọc cùng version ban đầu.
- Cả hai write đều được chấp nhận trong starter state.
- Blob cuối cùng không chứa đầy đủ hai thay đổi độc lập.
- Không có exception dù dữ liệu đã bị mất.

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

> Spoiler: chỉ xem sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước fix: chương trình in `LOST_UPDATE=True`.

Sau fix: stale write phải bị từ chối rõ ràng và blob đã commit trước đó phải được bảo toàn.

## Estimated Time

35–55 phút.

# UNIT-ES-002 — Bulk indexing reports success while documents are missing

## Mục tiêu
Điều tra một pipeline đồng bộ catalog sang Elasticsearch khi batch được báo thành công nhưng một số document không xuất hiện trong search index.

## Bối cảnh thực tế
Một worker gửi nhiều document trong một bulk request. Dashboard của worker ghi nhận batch thành công, nhưng đội vận hành phát hiện một số SKU không searchable. HTTP request hoàn tất bình thường và không có exception ở transport layer.

## Bạn cần làm gì
1. Chạy starter và reproduce triệu chứng.
2. Ghi hypothesis trước khi sửa.
3. Xác định evidence nào chứng minh batch thực sự có lỗi ở cấp document.
4. Sửa code trong `starter/` để kết quả phản ánh đúng trạng thái của toàn bộ batch.
5. Chạy `verify.ps1`.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script dùng response fixture cố định nên không cần Elasticsearch thật.

## Những gì cần quan sát
- Process kết thúc bình thường.
- Output của starter kết luận batch thành công.
- Fixture chứa kết quả chi tiết cho từng operation; hãy đối chiếu kết luận của ứng dụng với dữ liệu này.

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
[Reference Solution — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
**Before:** ứng dụng có thể kết luận `BATCH_OK` dù fixture thể hiện document không được index thành công.

**After:** ứng dụng phải phát hiện chính xác số operation thất bại và trả exit code khác 0 khi batch có partial failure.

## Estimated Time
35–50 phút.

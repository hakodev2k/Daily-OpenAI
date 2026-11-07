# UNIT-TEST-008 — Test suite fails around a time boundary

## Mục tiêu
Điều tra một test suite có kết quả phụ thuộc vào thời điểm thực thi và cải thiện design để business rule về thời gian có thể được kiểm chứng deterministically.

## Bối cảnh thực tế
Một subscription service quyết định entitlement còn hiệu lực hay đã hết hạn. Unit tests thường xanh nhưng thỉnh thoảng fail trên CI, đặc biệt với case sát expiration boundary. Rerun ngay sau đó có thể lại pass.

## Bạn cần làm gì
Reproduce failure deterministically bằng harness có sẵn, xác định boundary khiến production code và assertion không cùng quan sát một thời điểm, sửa `starter/`, rồi verify nhiều timestamps cố định.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Harness chạy business operation ở các timestamps được điều khiển để tái hiện discrepancy mà không cần chờ clock thật.

## Những gì cần quan sát
- Input business giống nhau nhưng result thay đổi khi observation time dịch chuyển.
- Test có đang sở hữu đầy đủ dependency quyết định kết quả hay không.
- Boundary semantics tại đúng expiration instant.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
Xem `hints/` theo thứ tự.

## Reference Solution
`solution/README.md` — spoiler warning.

## Expected Results
Sau fix, test kiểm soát được observation time, boundary rule rõ ràng và suite cho kết quả ổn định.

## Estimated Time
30–45 phút.
# UNIT-HASHKEY-001 — Manifest kiện hàng mất lịch sử quét sau đổi tuyến

## Mục tiêu
Điều tra sai lệch số lần quét trong một manifest bộ nhớ; sửa đúng hợp đồng định danh kiện hàng và chứng minh bằng regression checks.

## Bối cảnh thực tế
Trung tâm phân loại kiện hàng ghi nhận số lần quét của từng kiện. Điều phối viên có thể đổi tuyến sau lần quét đầu. Dashboard đôi lúc hiển thị 0 lượt quét; lần quét tiếp theo làm tăng số kiện theo dõi dù không có kiện mới. Các kiện không đổi tuyến hoạt động bình thường.

## Bạn cần làm gì
Reproduce lỗi, ghi ít nhất hai giả thuyết, đối chiếu dữ liệu trước/sau đổi tuyến, sửa **chỉ mã trong `starter/`**, chạy verify và ghi bằng chứng trước khi đọc solution.

## Yêu cầu môi trường
Windows PowerShell 5.1/7; .NET SDK 8+ hỗ trợ target `net8.0`. Không cần database, Docker, Azure hoặc NuGet package bên thứ ba.

## Chạy nhanh
Tại thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/run.ps1
```
Nếu execution policy không cho chạy file script, có thể dùng `powershell -File .\scripts\run.ps1` theo chính sách máy của bạn.

## Cách reproduce vấn đề
```powershell
./scripts/reproduce.ps1
```
Exit 0 nghĩa là **đã tái hiện đúng triệu chứng starter** (3 regression FAIL, 2 baseline PASS), không có nghĩa ứng dụng đã được sửa. Sau khi chỉnh `starter/`, chạy `./scripts/verify.ps1`.

## Những gì cần quan sát
Đối chiếu `GetScanCount`, `TrackedParcels`, cùng một kiện trước/sau đổi tuyến và hai scanner báo cùng mã kiện. Thu thập output và breakpoint evidence, không suy luận từ tên test đơn thuần.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ đọc sau khi tự sửa:** [Reference Solution](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
40 phút — L1 / D3. Không cần benchmark thời gian; kiểm tra các bất biến nghiệp vụ.

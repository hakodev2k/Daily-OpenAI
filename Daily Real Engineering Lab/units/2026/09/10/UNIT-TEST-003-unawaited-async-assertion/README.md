# UNIT-TEST-003 — Async assertion investigation

## Mục tiêu
Điều tra một regression test .NET có thể báo PASS dù assertion bất đồng bộ chưa hoàn tất.

## Bối cảnh thực tế
Một service gửi invoice sang provider bên ngoài. Failure path phải tạo `InvoiceDispatchException`, nhưng regression test hiện tại chưa chứng minh contract đó một cách đáng tin cậy.

## Bạn cần làm gì
1. Chạy `./reproduce.ps1`.
2. Ghi hypothesis về lifecycle của test method và assertion bất đồng bộ.
3. Sửa code trong `starter/`.
4. Chạy `./verify.ps1`.
5. Sau đó mới xem `solution/README.md`.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell

## Những gì cần quan sát
Quan sát thời điểm test method kết thúc, kiểu trả về của assertion API, và failure path có thực sự được runner quan sát hay không.

## Hints
- `hints/hint-01.md`
- `hints/hint-02.md`
- `hints/hint-03.md`

## Expected Results
Starter báo PASS dù async assertion chưa được buộc hoàn tất. Sau khi sửa, test runner phải chờ assertion và regression test phải phản ánh đúng exception contract.

## Estimated Time
25–40 phút.

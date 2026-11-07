# UNIT-MSET-001 — Đối soát số lượng bộ kit đồng hồ nước

## Mục tiêu
Reproduce sai lệch đối soát kho, thu thập bằng chứng, tự sửa và kiểm thử các trường hợp biên.

## Bối cảnh thực tế
Một kho cấp bộ kit thay đồng hồ nước theo manifest; máy quét ghi nhận từng bộ vật lý. Nhiều bộ có cùng SKU. Có chuyến giao thiếu vật tư nhưng báo cáo không ghi nhận chênh lệch. Nếu sai, kỹ thuật viên có thể xuất phát mà không đủ bộ kit. SKU phân biệt chữ hoa/chữ thường.

## Bạn cần làm gì
Chạy starter, ghi lại số lượng dự kiến/thực tế, đưa ra hai giả thuyết, điều tra `starter/Program.cs`, tự sửa `Reconciler.Reconcile` và kiểm thử trên starter đã sửa. Không chỉnh các fixture xác nhận hợp đồng.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7, .NET 8 SDK (có `global.json`). Không cần Docker, database hay NuGet ngoài BCL.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Trước khi sửa, `./reproduce.ps1` in `REPRODUCED` và exit 0 nếu triệu chứng được tái hiện. `./verify.ps1` phải thất bại trên starter ban đầu. Hai script có mục đích khác nhau.

## Những gì cần quan sát
So sánh số đơn vị vật lý trên manifest và bản quét; các mục `Missing` và `Unexpected` khi cùng SKU xuất hiện nhiều lần; thứ tự dòng khác nhau; khác biệt chữ hoa/chữ thường.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix ở starter.
4. Verify bằng `./verify.ps1`, exit 0 khi tất cả fixture pass.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) và [reference code](solution/Program.cs).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
40 phút; L1 / D3. Validation mode: static-review (không khẳng định đã thực thi .NET).

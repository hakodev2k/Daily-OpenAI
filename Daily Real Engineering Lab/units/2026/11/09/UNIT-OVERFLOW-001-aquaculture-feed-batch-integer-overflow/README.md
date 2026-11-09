# UNIT-OVERFLOW-001 — Sai lệch khối lượng lô thức ăn thủy sản

## Mục tiêu
Điều tra số gram tổng bị sai trong pipeline C#, thu thập bằng chứng và kiểm thử tại biên.

## Bối cảnh thực tế
Kho thức ăn thủy sản tổng hợp báo cáo từ số kiện và gram/kiện. Lô nhỏ khớp cân; lô lớn hợp lệ đôi khi cho khối lượng âm.

## Bạn cần làm gì
Reproduce, ghi ít nhất hai giả thuyết, xác định nguyên nhân, sửa đường tính toán trong starter và chạy verify.

## Yêu cầu môi trường
.NET SDK 8; PowerShell.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` tại thư mục unit; đối chiếu BATCH-900 với expected từ hệ thống cân.

## Những gì cần quan sát
Lưu count, gramsPerBag, actualGrams, expectedGrams; so sánh với lô nhỏ. Không sửa fixture.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[01](hints/hint-01.md) · [02](hints/hint-02.md) · [03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
35–50 phút.

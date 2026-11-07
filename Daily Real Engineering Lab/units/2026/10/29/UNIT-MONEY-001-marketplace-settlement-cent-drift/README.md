# UNIT-MONEY-001 — Sai lệch phí đến từng cent trong đối soát marketplace

## Mục tiêu

Điều tra một sai lệch tiền tệ nhỏ nhưng lặp lại giữa báo cáo settlement và sổ giao dịch. Tự tái hiện, thu thập evidence, xác định invariant bị vi phạm và sửa code mà không phá vỡ hoàn tiền hay phân tách seller.

## Bối cảnh thực tế

Một marketplace thủ công mỹ nghệ thanh toán cho nhiều seller mỗi ngày. Báo cáo tổng hợp hiển thị số tiền phí không khớp tổng phí của từng giao dịch đã được hạch toán. Sai lệch thường chỉ vài cent, xuất hiện nhiều hơn khi có các giao dịch giá trị thấp và refund. Bộ phận Finance tạm dừng xuất payout.

## Bạn cần làm gì

1. Đọc hợp đồng nghiệp vụ tại [docs/settlement-contract.md](docs/settlement-contract.md).
2. Chạy starter và tái hiện sai lệch bằng fixture có thể lặp lại.
3. Ghi evidence và ít nhất hai giả thuyết vào [workspace/my-investigation.md](workspace/my-investigation.md).
4. Sửa code trong `starter/Settlement.Core/SettlementCalculator.cs`.
5. Chạy verification, đối chiếu statement theo từng seller, bao gồm refund.
6. Chỉ sau đó mở reference solution.

## Yêu cầu môi trường

.NET SDK 8 hoặc SDK mới hơn có khả năng build `net8.0`; PowerShell 7 hoặc Windows PowerShell 5.1. Không cần Docker, NuGet bên ngoài, database hay cloud subscription.

## Chạy nhanh

Mở PowerShell tại thư mục unit:

```powershell
dotnet --version
./run.ps1
./reproduce.ps1
```

Nếu execution policy chặn script, dùng `pwsh -NoProfile -File ./reproduce.ps1` (PowerShell 7) hoặc chạy trực tiếp `dotnet run --project ./checks/Settlement.Checks/Settlement.Checks.csproj -- reproduce`.

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script trả exit code 0 khi nhận diện đúng triệu chứng đã biết ở starter; exit code khác 0 nếu không tái hiện được. Không chỉnh sửa starter trước lần reproduce đầu tiên.

## Những gì cần quan sát

So sánh gross, fee và net trong settlement với tổng các dòng giao dịch đã được ghi nhận. Kiểm tra một seller có ba khoản thu nhỏ, một seller khác có refund và trường hợp giao dịch lớn. Ghi số liệu thực tế, số liệu theo contract và mức chênh lệch; không suy đoán chỉ từ thông báo của script.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** [solution/README.md](solution/README.md) — chỉ xem sau khi tự sửa và verify.

## Expected Results

[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time

55 phút (reproduce 10, điều tra 15, fix 15, verify và review 15).

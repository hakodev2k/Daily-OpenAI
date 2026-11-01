# UNIT-JDUP-001 — Khoản hỗ trợ thiết bị thư viện bị ghi nhận sai hạn mức

## Mục tiêu

Điều tra một sai lệch giữa bước xét duyệt và bước ghi sổ từ cùng một HTTP JSON payload. Thu thập bằng chứng trước khi thay đổi logic, xây dựng một contract cho đầu vào không rõ ràng và bảo vệ các trường hợp hợp lệ.

## Bối cảnh thực tế

Hệ thống thư viện công cộng tự động duyệt khoản hỗ trợ thiết bị tối đa 5.000 đơn vị. Báo cáo kiểm toán cho thấy một yêu cầu đã được duyệt tự động nhưng số tiền được ghi nhận cao hơn giới hạn. Hai thành phần do hai nhóm phát triển cùng xử lý dữ liệu nhận vào. Không cần kết nối cơ sở dữ liệu: ứng dụng mô phỏng kết quả ghi sổ.

## Bạn cần làm gì

1. Chạy starter và ghi lại giá trị mà cổng xét duyệt cho phép và giá trị ghi sổ.
2. Đưa ra ít nhất hai hypothesis về nguyên nhân không nhất quán.
3. Điều tra dữ liệu đầu vào và hợp đồng giữa các thành phần.
4. Chỉnh sửa **starter/Program.cs**; không chỉnh sửa **checks/**.
5. Chạy verify và giải thích vì sao cả trường hợp bình thường lẫn trường hợp không rõ ràng đều được xử lý nhất quán.

## Yêu cầu môi trường

Windows PowerShell 7+ (hoặc Windows PowerShell 5.1), .NET 8 SDK tương thích `global.json`. Không cần Docker, Azure, database hay NuGet ngoài .NET SDK.

## Chạy nhanh

Tại thư mục unit:

```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

Nếu PowerShell chặn script, chạy từng lệnh bên trong script trong terminal được tin cậy; không cần thay đổi execution policy toàn hệ thống.

## Cách reproduce vấn đề

Trước khi sửa starter, chạy `./scripts/reproduce.ps1`. Lệnh trả exit code 0 khi triệu chứng ban đầu được tái hiện; in ra trạng thái xét duyệt và số tiền ghi sổ. Sau đó chạy `./scripts/verify.ps1` để thấy contract chưa được đáp ứng.

## Những gì cần quan sát

- Với yêu cầu thông thường, khoản hỗ trợ trong giới hạn được ghi đúng.
- Một payload kiểm toán có thể cho kết quả xét duyệt và ghi sổ không tương ứng.
- Contract runner kiểm tra cả dữ liệu hợp lệ, dữ liệu bất thường và xử lý lỗi.
- Sau khi sửa, mọi yêu cầu được chấp nhận phải có số tiền ghi sổ đúng bằng giá trị đã được kiểm tra.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify bằng `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ xem sau khi tự điều tra và sửa:** [Reference Solution](solution/README.md).

## Expected Results

[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md).

## Estimated Time

45–60 phút (L2 Investigation / D4).

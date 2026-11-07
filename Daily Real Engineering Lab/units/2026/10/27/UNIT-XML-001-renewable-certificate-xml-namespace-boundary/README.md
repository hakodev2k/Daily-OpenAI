# UNIT-XML-001 — Renewable Certificate XML: Lô được nhận nhưng tổng năng lượng bằng 0

## Mục tiêu

Điều tra một lỗi import XML âm thầm làm mất dữ liệu nghiệp vụ; thu thập bằng chứng từ payload và kết quả tính toán; sửa bộ chọn dữ liệu theo hợp đồng XML và viết regression checks cho dữ liệu của đối tác.

## Bối cảnh thực tế

Hệ thống đối soát chứng chỉ năng lượng tái tạo nhận một batch XML từ đối tác. Hệ thống ghi nhận batch `OCT-27` là đã xử lý nhưng báo `readingCount=0` và `totalKwh=0`. Đối tác khẳng định có hai số đo hợp lệ, tổng 20 kWh. Trong cùng payload còn có các phần tử phục vụ chẩn đoán, không được tính vào số liệu thanh toán.

## Bạn cần làm gì

1. Chạy starter và ghi nhận bằng chứng trước khi sửa.
2. Đề xuất ít nhất hai giả thuyết (payload sai, bộ chọn XML sai, định dạng số, hoặc scope dữ liệu sai).
3. Sửa duy nhất đường xử lý learner-editable `starter/CertificateLab/CertificateImporter.cs`.
4. Giữ nguyên định nghĩa batch và không đếm dữ liệu chẩn đoán hoặc dữ liệu thuộc hợp đồng khác.
5. Chạy verification; ghi kết luận và trade-offs vào `workspace/my-investigation.md`.

## Yêu cầu môi trường

- .NET 8 SDK hoặc SDK có khả năng build `net8.0`.
- PowerShell 7 hoặc Windows PowerShell; không cần Docker, cloud, NuGet bên thứ ba hay credential.

## Chạy nhanh

Từ thư mục unit:

```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

Sau khi tự sửa file trong `starter/`:

```powershell
./verify.ps1
```

Có thể chạy không cần PowerShell:

```bash
dotnet run --project starter/CertificateLab/CertificateLab.csproj -- inspect
dotnet run --project starter/CertificateLab/CertificateLab.csproj -- reproduce
dotnet run --project starter/CertificateLab/CertificateLab.csproj -- verify
```

## Cách reproduce vấn đề

`reproduce.ps1` chỉ thành công (exit code 0) khi nhận diện đúng **triệu chứng của starter chưa sửa**. Sau khi sửa đúng, lệnh này dự kiến trả exit code 1 vì lỗi không còn tái hiện. `verify.ps1` mới là bước xác nhận bản sửa (exit code 0 khi pass).

## Những gì cần quan sát

- Batch được nhận nhưng số bản ghi và tổng năng lượng không khớp với chứng từ đối tác.
- Hai cách biểu diễn payload tương đương cần tạo ra kết quả tương đương.
- Dữ liệu chẩn đoán không được cộng vào số liệu đối soát.
- Payload không tuân thủ hợp đồng phải bị từ chối rõ ràng, không trả kết quả thành công rỗng.
- Xem `evidence/incident-notes.md` để phân biệt bằng chứng với giả thuyết.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis và bằng chứng.
3. Thử fix trên `starter/`.
4. Verify hành vi, kể cả negative cases.
5. Chỉ sau đó mới xem reference solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** chỉ mở sau khi tự thử: [Giải pháp tham khảo](solution/README.md).

## Expected Results

[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time

50–60 phút (L2 Investigation, D4). Phần mở rộng Senior là tùy chọn.

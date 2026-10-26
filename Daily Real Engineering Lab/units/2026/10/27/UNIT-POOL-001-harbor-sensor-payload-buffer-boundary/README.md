# UNIT-POOL-001 — Harbor sensor payload: kiểm tra độ dài dữ liệu

## Mục tiêu

Tái hiện và điều tra sai lệch độ dài giữa payload thiết bị gửi và dữ liệu đối tác nhận. Thu thập bằng chứng, tự sửa starter và chứng minh contract đúng ở các kích thước biên.

## Bối cảnh thực tế

Một gateway tại cảng chuyển tiếp cảnh báo hàng hải dưới dạng Base64 trong JSON. Đối tác yêu cầu số bytes sau giải mã đúng bằng declaredLength. Gói 32 bytes thường được chấp nhận, nhưng gói ngắn bị báo length mismatch và có thể chứa bytes không thuộc thông điệp. Không được bỏ qua yêu cầu hiệu năng và hoàn trả tài nguyên.

## Bạn cần làm gì

1. Chạy reproduce trước khi sửa.
2. Ghi ít nhất hai hypothesis và đối chiếu evidence.
3. Chỉ sửa **starter/PacketPayloadEncoder.cs**; không sửa harness trong Program.cs.
4. Chạy verify và giải thích vì sao case gói đầy có thể tạo false confidence.
5. Đọc reference solution sau khi tự hoàn tất.

## Yêu cầu môi trường

.NET SDK 8.0.x và PowerShell 5.1+ hoặc PowerShell 7. Không cần Docker, Azure hay NuGet package ngoài framework.

## Chạy nhanh

~~~powershell
dotnet --version
dotnet restore ./starter/TelemetryRelay.csproj
./run.ps1
./reproduce.ps1
~~~

## Cách reproduce vấn đề

~~~powershell
./reproduce.ps1
~~~

Script này trả thành công khi **phát hiện triệu chứng trong starter chưa sửa**. Đây không phải bước chứng nhận fix.

## Những gì cần quan sát

- Declared payload length so với decoded outbound length.
- Khác biệt giữa gói ngắn, rỗng và gói đủ 32 bytes.
- Số lần trả tài nguyên tạm.
- Các hypothesis có thể giải thích bằng chứng, thay vì chỉ sửa cho test pass.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Reference solution](solution/README.md) — chỉ mở sau khi tự sửa và verify.

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time

35 phút. [Investigation workspace](workspace/my-investigation.md).

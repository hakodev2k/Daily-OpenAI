# UNIT-ID-001 — Export Permit Registry: kết quả đối soát khác nhau giữa các worker

## Mục tiêu

Điều tra một lỗi đối soát dữ liệu xảy ra có chọn lọc giữa các môi trường chạy .NET 8. Thu thập evidence, xây dựng hypothesis, sửa code và kiểm chứng **tính nhất quán của identity contract** mà không làm thay đổi quy tắc cập nhật trạng thái hợp lệ.

## Bối cảnh thực tế

Dịch vụ nhập chứng nhận kiểm dịch nông sản nhận nhiều revision cho cùng mã giấy phép từ đối tác. Theo hợp đồng tích hợp, mã giấy phép là **ASCII technical identifier**, không phân biệt chữ hoa/chữ thường; revision đến sau phải thay thế revision trước. Một số worker báo 3 giấy phép sau khi nhận 3 bản ghi, trong khi worker khác chỉ báo 2. Màn hình kiểm tra đôi khi vẫn hiển thị trạng thái cũ.

## Bạn cần làm gì

1. Chạy starter và ghi lại kết quả của hai môi trường mô phỏng.
2. Phân biệt lỗi nhập dữ liệu, lỗi thứ tự feed và lỗi xử lý identity.
3. Đưa ra hypothesis có thể kiểm chứng; sửa `starter/` mà không thay đổi dữ liệu mẫu.
4. Chạy verification, xác nhận revision cuối cùng và hai mã khác nhau không bị nhập làm một.
5. Chỉ sau đó đọc reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x trở lên có thể build target `net8.0`; PowerShell 7 hoặc Windows PowerShell 5.1.
- Globalization data mặc định của .NET phải khả dụng; không bật `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT`.
- Không cần database, Docker, cloud, NuGet package bên ngoài hay credentials.

## Chạy nhanh

Từ thư mục unit:

~~~powershell
./run.ps1
./reproduce.ps1
~~~

Có thể dùng lệnh tương đương:

~~~powershell
dotnet run --project ./starter/PermitRegistryLab.csproj -- --diagnose
dotnet run --project ./starter/PermitRegistryLab.csproj -- --reproduce
~~~

## Cách reproduce vấn đề

`reproduce.ps1` phải exit 0 **khi xác nhận được symptom của starter**; nó không xác nhận hệ thống đã đúng. Quan sát count và trạng thái `INSP-204` trong hai môi trường mô phỏng. Không sửa data seed hoặc kiểm tra bằng cách bỏ bớt revision.

## Những gì cần quan sát

- Hai worker dùng cùng thứ tự input và cùng binary nhưng báo số lượng record khác nhau.
- Trạng thái trả về cho `INSP-204` không đồng nhất.
- Mã `INSP-205` vẫn phải được giữ độc lập.
- Kết quả của `--verify` trước và sau khi sửa.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Reference Solution](solution/README.md) — chỉ xem sau khi tự điều tra và sửa.

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time

45–60 phút. Ghi evidence và hypothesis vào [workspace](workspace/my-investigation.md).

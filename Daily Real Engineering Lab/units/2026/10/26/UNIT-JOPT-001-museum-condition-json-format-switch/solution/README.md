# Reference Solution — chỉ xem sau khi tự sửa

## Symptoms

JSON compact thành công, lượt pretty tiếp theo ném <code>InvalidOperationException</code> trong cùng worker. Process mới cho mỗi request có thể che giấu triệu chứng.

## Evidence

<code>--reproduce</code> xuất báo cáo hai lượt, input hợp lệ. Exception xuất hiện khi chuyển format, trước khi có JSON thứ hai. <code>--verify</code> kiểm tra bốn lượt liên tiếp.

## Root cause

Sau lần đầu sử dụng <code>JsonSerializer.Serialize</code>, <code>JsonSerializerOptions</code> trở thành read-only. Starter giữ một instance tĩnh và gán <code>WriteIndented</code> trên mỗi request. Thao tác mutate sau khi serializer dùng options là không hợp lệ, kể cả khi gán cùng giá trị.

## Why the fix works

Tạo sẵn hai cấu hình riêng (compact và pretty) **trước** khi dùng, chỉ chọn và đọc chúng tại request path. Không mutate state được chia sẻ.

Mã tham chiếu: [ConditionReportExporter.cs](ConditionReportExporter.cs). Để so sánh sau khi tự thử:

~~~powershell
Copy-Item ./solution/ConditionReportExporter.cs ./starter/ConditionReportExporter.cs -Force
./verify.ps1
~~~

## How to verify

<code>PASS</code>, exit code 0; bốn lượt xen kẽ không lỗi, JSON giữ camelCase và dữ liệu đúng, chỉ whitespace thay đổi.

## Alternative fixes

- Tạo options mới mỗi request: đúng nhưng tốn allocations và mất lợi ích cache metadata.
- Inject hai profile bất biến qua DI: tốt khi số profile lớn hoặc cần test cấu hình riêng.
- Cấu hình đầy đủ và chủ động đóng băng options sớm: phù hợp nếu ứng dụng cần fail fast; vẫn phải bảo đảm resolver tương thích runtime.

## Wrong / tempting fixes

- <code>lock</code> quanh shared options không thay đổi tính read-only.
- Catch exception rồi trả payload trước đó gây sai dữ liệu.
- Luôn dùng pretty làm hỏng compact contract.
- Retry sau exception mà không thay lifecycle chỉ lặp lại lỗi.

## Production implications

Static/singleton helpers tồn tại xuyên nhiều HTTP requests; cần test chuỗi request trong cùng process, không chỉ test đơn lẻ. Cấu hình serialization phải được khởi tạo trước traffic, không mutate theo tenant/request. Với format động, phải kiểm soát số lượng cached profiles.

## What a Senior engineer should notice

Phân biệt lỗi lifecycle với lỗi input, cold start với steady state, và xác nhận contract bằng behavioral tests. Tính đúng đắn và thread safety quan trọng hơn chỉ làm biến mất exception.

[Concepts after lab](../docs/concepts-after-lab.md).

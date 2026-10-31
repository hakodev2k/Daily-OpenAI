# UNIT-URLAUTH-001 — Callback telemetry đập thủy điện vượt qua kiểm tra địa chỉ tin cậy

## Mục tiêu
Điều tra chênh lệch giữa policy callback và kết quả hệ thống; phân tích evidence, lập giả thuyết, sửa ranh giới tin cậy và kiểm tra regression độc lập.

## Bối cảnh thực tế
Nền tảng giám sát đập thủy điện cho phép đăng ký callback tới gateway đối tác. Chỉ URL HTTPS trên host \`hooks.damtelemetry.example\`, cổng 443, tại route \`/ingest\` hoặc \`/ingest/v2\` được chấp nhận. Query string dùng cho mã trạm được phép; credentials và fragment bị cấm. Một số URL ngoài policy vẫn qua vòng kiểm tra. Các domain chỉ là ví dụ, không có network request.

## Bạn cần làm gì
1. Reproduce và ghi lại những URL được duyệt sai.
2. Tách scheme, authority, host, port, userinfo, path, query, fragment và đưa ra ít nhất hai hypotheses.
3. Chỉ sửa \`starter/CallbackPolicy.cs\`; không sửa fixtures, verification hoặc scripts.
4. Bảo toàn callback hợp lệ, từ chối tất cả callback trái policy.
5. Verify trên bộ case độc lập trước khi mở reference.

## Yêu cầu môi trường
PowerShell 5.1+ hoặc 7, .NET SDK 8.0.x; không cần Docker, database, Azure hay NuGet package ngoài.

## Chạy nhanh
~~~powershell
cd 'Daily Real Engineering Lab/units/2026/11/01/UNIT-URLAUTH-001-dam-telemetry-callback-authority-confusion'
./setup.ps1
./run.ps1        # starter dự kiến exit code 2
./reproduce.ps1  # thành công nếu chứng minh lỗi starter
~~~

## Cách reproduce vấn đề
Chạy \`./reproduce.ps1\` **trước khi chỉnh sửa**; script chỉ chứng minh symptom ban đầu. Sau khi sửa \`starter/CallbackPolicy.cs\`, chạy \`./verify.ps1\`. Verification biên dịch trực tiếp file learner đã sửa và không dùng solution.

## Những gì cần quan sát
So sánh expected/actual của từng fixture FAIL, phân loại host, route, port và các thành phần URL bị cấm. Chương trình chỉ in kết quả; không gọi HTTP.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix.
4. Verify độc lập.
5. Chỉ sau đó xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ xem sau khi tự làm.

## Expected Results
Trước: \`run.ps1\` exit 2, có mismatch; \`reproduce.ps1\` xác nhận. Sau: \`verify.ps1\` exit 0, cả 15 case độc lập PASS. Không phụ thuộc thời gian hoặc Internet.

## Estimated Time
45–60 phút.

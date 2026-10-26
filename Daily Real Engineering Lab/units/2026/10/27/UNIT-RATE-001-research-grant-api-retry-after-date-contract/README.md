# UNIT-RATE-001 — Retry-After tại cổng nộp hồ sơ nghiên cứu

## Mục tiêu

Điều tra và refactor chính sách chờ sau HTTP 429 của .NET partner integration. Giữ các contract đã đúng, xác định failure mode bằng evidence, rồi bổ sung regression coverage.

## Bối cảnh thực tế

Trong đợt cao điểm xét duyệt học bổng nghiên cứu, cổng nộp hồ sơ gọi dịch vụ kiểm tra tư cách đối tác. Một số phản hồi HTTP 429 dẫn tới client retry quá sớm dù đối tác đã cung cấp chỉ dẫn chờ. Các request khác không gặp vấn đề. Retry storm khiến thời gian throttle kéo dài.

## Bạn cần làm gì

1. Chạy baseline và ghi nhận fixture thành công/thất bại.
2. Ghi ít nhất hai hypothesis, đối chiếu evidence và quan sát object header.
3. Refactor duy nhất \`starter/RetryPlanner.cs\`, giữ nguyên method signature.
4. Chạy verify, chứng minh đủ tám fixture và các boundary cases.
5. Đánh giá rủi ro khi chuyển từ simulator sang production HTTP client.

## Yêu cầu môi trường

.NET 8 SDK, PowerShell 7+ hoặc Windows PowerShell có dotnet CLI. Không cần NuGet package, Docker, Internet hoặc Azure. SDK được pin bằng \`global.json\`.

## Chạy nhanh

\`\`\`powershell
./reproduce.ps1
# Tự sửa starter/RetryPlanner.cs
./verify.ps1
\`\`\`

Dùng \`./run.ps1\` để xem chi tiết baseline; harness có thể trả exit code 12 khi còn mismatch.

## Cách reproduce vấn đề

\`reproduce.ps1\` chạy **starter** và chỉ xác nhận symptom nếu harness trả đúng exit code 12. Build failure hoặc setup error không được tính là reproduce thành công.

## Những gì cần quan sát

- Fixture nào đúng, fixture nào sai? Phân nhóm theo raw response header.
- Chênh lệch giữa thời gian chờ yêu cầu và thời gian được lập lịch.
- Fallback 2 giây, maxDelay 30 giây, và trường hợp thời gian đã qua.
- CPU thấp không đủ chứng minh chính sách retry đúng.

Xem \`evidence/incident-timeline.md\` và ghi nhận vào \`workspace/my-investigation.md\`.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ mở [solution/README.md](solution/README.md) sau khi đã tự điều tra.

## Expected Results

Trước sửa: build thành công nhưng fixture contract chưa đạt, exit code **12**. Sau sửa: **8/8** fixture đạt, exit code **0**. Đồng hồ cố định trong fixture; không dùng sleep hay benchmark phụ thuộc tốc độ máy.

## Estimated Time

45–65 phút. Maturity L4, difficulty D4.

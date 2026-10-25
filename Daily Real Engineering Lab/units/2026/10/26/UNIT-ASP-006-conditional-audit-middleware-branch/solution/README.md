# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Hai route API 404 dù health 200; audit header vẫn xuất hiện.

## 2. Evidence
Request `/api` đã đi qua audit middleware, nhưng body của business handler không xuất hiện. Health không chịu ảnh hưởng.

## 3. Root cause
`MapWhen` tạo **branch pipeline riêng**. `next` trong audit middleware tiếp tục trong branch, sau đó default terminal delegate trả 404. Nó không quay về `app.Run` của main pipeline.

## 4. Why the fix works
Mục tiêu chỉ là conditional cross-cutting middleware: dùng `UseWhen` để nhánh rejoin main pipeline khi không terminal. `solution/Program.cs` đổi duy nhất branch construction; handler và behavioral assertions giữ nguyên.

## 5. How to verify
Sửa `starter/Program.cs`, chạy `pwsh -File ./scripts/verify.ps1` và quan sát `PASS` / exit 0. Có thể chạy `dotnet run --project ./solution/PipelineLab.csproj -- verify` để tự kiểm reference, nhưng không thay learner verification.

## 6. Alternative fixes
Một `app.Use` với predicate nội bộ cũng hợp lý nếu condition đơn giản. `MapWhen` vẫn đúng khi chủ đích xây terminal branch có endpoints riêng.

## 7. Wrong / tempting fixes
- Hard-code 200 trong audit branch: bỏ qua business handler.
- Xóa audit policy: vi phạm yêu cầu vận hành.
- Đặt terminal `app.Run` trước audit: audit không chạy.
- Client retry: 404 deterministic không phải transient fault.

## 8. Production implications
Health xanh không chứng minh business endpoints hoạt động. Canary smoke test phải kiểm tra route thật, body, header và negative paths.

## 9. Trade-offs
`UseWhen` thích hợp cho middleware có điều kiện cần rejoin; `MapWhen` cho nhánh độc lập. Không thay toàn bộ `MapWhen` một cách máy móc.

## 10. Senior engineer should notice
Phân biệt “đã đi qua middleware” và “đã tới handler”. Khi deploy middleware làm route hỏng, vẽ control flow và kiểm thử boundary trước khi tăng retry hay rollback.

## Official reference
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0

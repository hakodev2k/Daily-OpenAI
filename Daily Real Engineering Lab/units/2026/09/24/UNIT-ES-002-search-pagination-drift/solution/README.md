# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## Symptoms
Các page riêng lẻ hợp lệ nhưng toàn bộ pagination session có duplicate/missing ticket khi index thay đổi giữa requests.

## Evidence
Page 1 được tính trước mutation; page 2 được tính từ một ordering mới sau mutation. Offset 10 vì thế không còn biểu diễn ranh giới của result set mà page 1 đã nhìn thấy.

## Root cause
Offset pagination (`from`/`size`) chạy các search độc lập trên một result set đang thay đổi. Mutation có thể đưa document từ phía sau lên trước boundary, làm các document khác dịch vị trí. Nếu sort field không unique, boundary còn có thể thiếu deterministic tie-breaker.

## Why the fix works
Với Elasticsearch, một hướng production phù hợp là mở Point in Time cho pagination session và dùng `search_after` dựa trên sort values của item cuối trang trước, với deterministic sort/tie-breaker. PIT giữ một consistent search view trong session; cursor tránh phụ thuộc vào offset bị dịch chuyển.

Trong simulator của lab, learner có thể mô hình hóa cùng contract bằng snapshot bất biến của ordered IDs cho session và cursor theo item cuối, thay vì tính lại offset trên collection mutable.

## How to verify
Giữ mutation hoạt động, sửa `starter/`, chạy `./verify.ps1`. Hai page đầu phải có 20 ID unique và không duplicate.

## Alternative fixes
- Nếu UX chỉ cần eventual browsing và chấp nhận drift, có thể giữ offset nhưng phải document contract rõ ràng.
- Keyset/cursor pagination trên một data source có stable ordering phù hợp có thể đủ khi không cần snapshot consistency.
- Với export lớn, dùng workflow/background snapshot riêng thay vì biến interactive search thành export protocol.

## Wrong / Tempting Fixes
- Tăng page size chỉ làm thay đổi xác suất/điểm boundary.
- `Distinct()` ở client che duplicate nhưng không phục hồi item bị missing.
- Retry page 2 có thể nhìn thấy một state khác nữa.
- Chỉ thêm secondary sort làm ordering deterministic nhưng không tự tạo snapshot consistency giữa hai independent searches.

## Production implications
PIT có lifetime và resource cost; cursor cần được coi là opaque API contract; cần xử lý expired PIT, query/filter changes giữa session, và giới hạn session duration.

## Trade-offs
Snapshot-style pagination cho UX ổn định hơn nhưng tăng state/resource lifecycle. Offset đơn giản và phù hợp với tập nhỏ, ít mutation hoặc use case không yêu cầu cross-page consistency.

## What a Senior engineer should notice
Pagination không chỉ là `pageNumber/pageSize`; nó là consistency contract giữa nhiều requests. Cần quyết định rõ semantics khi dữ liệu thay đổi, rồi chọn cơ chế tương ứng.
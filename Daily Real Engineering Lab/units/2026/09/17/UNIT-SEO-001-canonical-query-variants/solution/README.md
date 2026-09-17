# Reference Solution — chỉ xem sau khi đã reproduce và tự thử

## 1. Symptoms
Cùng product resource xuất hiện với nhiều canonical identities khi request chứa tracking hoặc presentation query parameters.

## 2. Evidence
Starter nối toàn bộ query string vào canonical URL. Vì vậy bất kỳ query dimension nào cũng trở thành một phần của identity mà crawler quan sát.

## 3. Root cause
Canonical-generation policy không phân biệt request dimensions phục vụ tracking/presentation với dimensions thực sự định danh resource.

## 4. Why the fix works
Reference policy bắt đầu từ stable path identity, loại bỏ non-resource dimensions và chỉ giữ `variant` vì business contract của lab định nghĩa variant là resource-defining.

## 5. How to verify
Chạy `verify.ps1` trên learner-editable `starter/`. Tracking và sorting samples phải hội tụ về product path; variant samples phải giữ `variant=blue` nhưng không giữ `utm_*`.

## 6. Alternative fixes
Trong production, policy có thể nằm ở routing/content model thay vì parse URL ad-hoc. Với CMS/commerce platform, canonical URL thường nên được tạo từ content identity và localization/variant rules đã biết.

## 7. Wrong / tempting fixes
- Xóa mọi query parameter: có thể gộp nhầm resource variants thực sự khác nhau.
- Giữ mọi query parameter: duy trì duplicate signals.
- Hard-code duy nhất `utm_source`: bỏ sót các tracking dimensions khác và không thể hiện identity contract.

## 8. Production implications
Canonical policy cần thống nhất với routing, sitemap, internal links, hreflang/locale strategy và redirects. Canonical không thay thế access control hay URL normalization server-side.

## 9. Trade-offs
Allowlist resource-defining dimensions dễ audit nhưng cần cập nhật khi business thêm variant mới. Generic filtering ít maintenance hơn nhưng dễ phân loại sai semantics.

## 10. Senior insight
Đừng coi canonicalization là string cleanup. Đây là quyết định về resource identity: hệ thống phải xác định dimensions nào làm thay đổi nội dung đại diện và dimensions nào chỉ mô tả cách request đến resource.
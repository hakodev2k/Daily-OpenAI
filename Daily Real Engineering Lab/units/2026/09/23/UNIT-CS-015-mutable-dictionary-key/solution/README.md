# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Lookup thành công ngay sau insertion nhưng thất bại sau khi `DiscountPercent` thay đổi, trong khi cùng object vẫn nằm trong dictionary.

## 2. Evidence
`before=true`, `after=false`, `count=1`, và enumeration xác nhận cùng reference vẫn tồn tại. Giá trị hash của key thay đổi khi business state thay đổi.

## 3. Root cause
`Dictionary<TKey,TValue>` đặt entry vào bucket dựa trên hash tại thời điểm insertion. `PromotionRule.GetHashCode()` và equality lại phụ thuộc `DiscountPercent`, một property mutable. Sau mutation, lookup tính hash mới và tìm bucket khác với bucket chứa entry.

## 4. Why the fix works
Identity dùng cho hash membership được giới hạn vào `Code`, một giá trị ổn định trong lifetime của rule. Business state vẫn có thể thay đổi mà không phá invariant của hash-based collection.

## 5. How to verify
Áp dụng fix tương đương vào `starter/Program.cs`, chạy `./verify.ps1`; cần thấy `VERIFY_OK`.

## 6. Alternative fixes
- Dùng immutable value key riêng như `RuleId`/`Code` thay vì cả entity.
- Remove key trước mutation rồi add lại sau mutation nếu semantics thực sự yêu cầu toàn bộ value tham gia identity; cách này dễ sai khi mutation có nhiều đường đi.

## 7. Wrong or misleading fixes
- Đổi sang linear scan chỉ che lỗi invariant và làm lookup O(n).
- Reinsert ngẫu nhiên khi lookup fail che symptom nhưng không xác định ownership của identity.
- Dùng reference equality có thể hợp lệ trong một số object-graph nội bộ, nhưng thay đổi semantics nếu domain cần logical identity.

## 8. Production implications
Lỗi này có thể tạo cache miss giả, duplicate processing hoặc state divergence khó reproduce vì phụ thuộc thời điểm mutation.

## 9. Trade-offs
Stable business key đơn giản và nhanh nhưng phải có uniqueness contract. Immutable key object tách identity rõ hơn nhưng thêm type/API surface.

## 10. What a Senior engineer should notice
Hash-based collection có invariant về equality/hash stability trong thời gian key là member. Domain equality và mutable business state không nên được trộn tùy tiện; cần quyết định identity boundary rõ ràng.
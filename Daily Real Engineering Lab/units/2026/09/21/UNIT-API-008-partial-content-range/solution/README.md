# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Request gần cuối resource nhận `206`, body chứa đúng các byte còn lại, nhưng `Content-Range` mô tả một interval dài hơn representation thực tế.

## 2. Evidence

Với resource 100 bytes và request `90-120`, body chỉ có 10 bytes (`90..99`) trong khi starter quảng bá `bytes 90-120/100`.

## 3. Root cause

Code đã clamp vị trí cuối khi cắt payload nhưng xây `Content-Range` từ `requestedEnd` chưa được normalize. Payload và protocol metadata vì vậy dùng hai boundary khác nhau.

## 4. Why the fix works

Dùng cùng `actualEnd` đã giới hạn bởi `resource.Length - 1` cho cả payload và `Content-Range`, nên response mô tả chính xác representation bytes được gửi.

## 5. How to verify

Chạy `./verify.ps1` trên learner-editable `starter/`. Verification kiểm tra range bình thường, range vượt cuối resource và single-byte tail.

## 6. Alternative fixes

Có thể đóng gói việc normalize range thành một value object/helper và chỉ cho response builder nhận normalized range. Với ASP.NET Core thực tế, ưu tiên framework primitives hỗ trợ range processing khi phù hợp thay vì tự triển khai toàn bộ RFC semantics.

## 7. Wrong or misleading fixes

Không nên padding body để khớp requested end: đó không còn là bytes của representation. Không nên đổi mọi response thành `200`: cách đó loại bỏ resume semantics thay vì sửa contract. Chỉ sửa body length mà giữ header sai cũng không giải quyết interoperability.

## 8. Production implications

Protocol metadata sai có thể chỉ xuất hiện với một số client nghiêm ngặt, CDN hoặc download manager. Vì vậy API tests nên kiểm tra invariant giữa status, headers, representation length và payload, không chỉ kiểm tra body.

## 9. Trade-offs

Tự implement range logic cho phép custom behavior nhưng kéo theo nhiều edge cases như suffix ranges, multiple ranges, unsatisfied ranges và representation changes. Framework implementation thường giảm protocol risk.

## 10. What a Senior engineer should notice

Bug nằm ở contract consistency: cùng một normalized boundary phải chi phối mọi phần của response. Khi một request được transform/normalized, đừng để raw input tiếp tục rò vào downstream metadata.
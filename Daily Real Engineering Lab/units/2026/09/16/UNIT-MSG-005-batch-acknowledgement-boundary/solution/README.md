# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms
`m1` và `m2` tạo side effect thành công ở attempt đầu. `m3` thất bại. Ở attempt sau, broker delivery lại cả batch nên side effect của `m1` và `m2` tăng lên lần hai.

## 2. Evidence
Log cho thấy `ATTEMPT 2` vẫn chứa các message đã xử lý trước failure. Side-effect count là bằng chứng trực tiếp rằng application progress và broker completion progress đã lệch nhau.

## 3. Root cause
Consumer chỉ gọi completion cho toàn batch sau khi toàn bộ vòng lặp thành công. Khi một item giữa batch throw exception, completion progress của các item trước đó không được ghi nhận dù business side effect của chúng đã xảy ra.

## 4. Why the fix works
Trong simulator này, ghi nhận completion ngay sau từng item thành công khiến broker loại item đó khỏi lần delivery sau. Failure của một item không xóa completion progress của các item trước.

## 5. How to verify
Sửa `starter/` rồi chạy `./verify.ps1`. `order-101` và `order-102` phải giữ count bằng 1 qua hai attempts.

## 6. Alternative fixes
- Xử lý từng message độc lập thay vì batch nếu throughput vẫn đáp ứng.
- Dùng inbox/idempotency record để business handler có thể chịu redelivery.
- Với broker hỗ trợ settlement riêng từng message, settle item thành công và áp dụng retry/dead-letter policy cho item lỗi.

## 7. Wrong or misleading fixes
- Catch exception rồi tiếp tục nhưng không thay đổi completion semantics: có thể làm mất failure visibility mà duplicate vẫn còn.
- Tăng retry delay: chỉ thay đổi thời điểm duplicate xảy ra.
- Giả định broker sẽ delivery exactly once: không phải contract an toàn cho đa số messaging systems.

## 8. Production implications
Trong hệ thống thật, acknowledgement và business transaction thường không atomic. Process có thể crash sau database commit nhưng trước broker settlement. Vì vậy per-message settlement giảm blast radius của batch failure nhưng không loại bỏ nhu cầu idempotency cho side effects quan trọng.

## 9. Trade-offs
Per-message acknowledgement tăng số broker operations và có thể giảm throughput. Batch settlement hiệu quả hơn nhưng cần failure isolation/idempotency mạnh hơn. Chọn boundary dựa trên cost của duplicate, throughput và broker semantics.

## 10. What a Senior engineer should notice
Bài toán không chỉ là `try/catch`. Cần vẽ rõ các durability boundaries: khi nào business state durable, khi nào broker state durable, điều gì xảy ra nếu process chết giữa hai mốc. Một consumer đáng tin cậy phải thiết kế cho redelivery thay vì xem redelivery là exception hiếm.
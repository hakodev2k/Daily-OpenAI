# Bài toán
ASP.NET Core API xác thực công dân qua Entra ID, kiểm quyền trong SQL rồi trả direct Azure Blob SAS URL (private container) TTL 30 phút. File 2–80 MB; 1,000 downloads/min; API cấp link P99 <300ms.
**Contract:** Standard document: mọi GET mới bắt đầu sau khi revoke commit 60 giây phải bị từ chối. Sensitive document: mọi GET mới sau revoke phải bị từ chối ngay. GET đã bắt đầu trước revoke được phép hoàn tất. Audit phải có actor, documentId, policyVersion, requestId, decision; không log SAS.
**Nhiệm vụ:** Vẽ trust boundaries và sequence cấp link/revoke/GET. Lập bảng so sánh direct SAS, gateway, hybrid theo revocation, performance, egress, availability, audit và vận hành. Nêu cơ chế kiểm soát cache/CDN, Range/206, token leakage, key rotation, policy caching và fail-closed. Thiết kế staged rollout và rollback.
**Change A:** Standard SLA chuyển thành 10 giây nhưng vẫn muốn direct Blob URL. Chứng minh được hay đề xuất thay đổi.
**Change B:** Sensitive link bị chia sẻ sang máy khác; phải chặn ngay ở GET mới.
Không tự suy ra rằng DB revoke sẽ vô hiệu hóa token đã cấp.

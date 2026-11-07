# Hint 3 — Cơ chế và rollback

Hãy thử tách migration thành **expand**, **compatibility app rollout**, **backfill**, **read cutover**, **contract**. Nếu phải rollback sau read cutover, phiên bản nào hiểu cả hai cột và vẫn bảo toàn writes? Chỉ drop cột khi không còn code hoặc rollback artifact nào phụ thuộc vào nó.

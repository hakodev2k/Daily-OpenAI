# Senior extension
Thiết kế contract `TryAcceptAsync` có timeout, cancellation và explicit rejection; mô phỏng consumer ngừng 2 giây. Phân tích tại sao `Wait` không bảo đảm durability khi process bị kill, rồi so sánh in-memory channel với transactional outbox. Đề xuất metric có cardinality hữu hạn và shutdown drain policy.

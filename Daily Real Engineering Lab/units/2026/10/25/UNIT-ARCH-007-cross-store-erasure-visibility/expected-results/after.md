# Expected results / self-review rubric

Đây là design case, không có output console cố định. Tự chấm 0–2 điểm mỗi mục (tối đa 20):

1. Mô tả chính xác user-visible failure, không chỉ một component error.
2. Phân biệt SQL authority, search projection, cache, Blob URL và backup.
3. Định nghĩa invariant public visibility sau ACK, theo tenant + record.
4. Thiết kế xử lý stale reindex / event ordering / idempotent retry.
5. Phân biệt public suppression và physical erasure/legal hold.
6. Đánh giá link đã phát hành và giới hạn revoke thực tế.
7. Có safe recovery sau worker crash và backup restore.
8. Đánh đổi p95 search, availability và fail-closed behavior.
9. So sánh ít nhất ba phương án theo cùng constraints.
10. Có test oracle cho AC-01…AC-12, metric, owner, rollout/rollback.

**Đạt:** 16–20 điểm và không vi phạm AC-01/AC-06/AC-08 về lời hứa sau ACK. **Cần rework:** thiếu invariant hoặc cam kết xoá ngay mọi backup/link mà không có cơ chế khả thi. Dưới 16 điểm: cải thiện ADR trước khi xem solution.

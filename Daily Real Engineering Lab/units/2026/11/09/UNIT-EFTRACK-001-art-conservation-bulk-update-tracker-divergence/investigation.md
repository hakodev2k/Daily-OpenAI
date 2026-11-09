# Investigation worksheet
1. Ghi lại `dbAfterBulk`, `tracked`, `finalPriority`, `finalNote`.
2. Giả thuyết A: transaction rollback; giả thuyết B: một write sau đó ghi đè; giả thuyết C: truy vấn bị cache.
3. Chọn quan sát phân biệt A/B/C; xác định thời điểm divergence.
4. Sửa chỉ phần learner-editable, chạy verify và ghi lại output.
5. So sánh minimal fix, reload, và tách DbContext theo chi phí query và rủi ro concurrent update.

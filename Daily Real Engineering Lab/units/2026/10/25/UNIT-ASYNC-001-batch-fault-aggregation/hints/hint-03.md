# Hint 03 — Concept
Khi await Task.WhenAll trên nhóm có nhiều task faulted, await có thể ném một exception trong khi Task tổng hợp giữ nhiều lỗi. Phân biệt Faulted, RanToCompletion và Canceled; đừng gộp cancellation vào lỗi đối tác.

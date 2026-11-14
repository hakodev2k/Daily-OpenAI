# Hints — mở lần lượt

1. So sánh số phần tử trong tập đầu vào và đầu ra khi hai lô có cùng ngày hết hạn.
2. Xem contract của `SortedSet<T>` và ý nghĩa giá trị 0 từ comparer.
3. Một phép so sánh có thể đồng thời quyết định thứ tự **và** tính tương đương trong collection này. Hãy xác định tiêu chí định danh lô.
4. Dùng tie-breaker ổn định bằng BatchId và viết policy riêng cho trùng BatchId.

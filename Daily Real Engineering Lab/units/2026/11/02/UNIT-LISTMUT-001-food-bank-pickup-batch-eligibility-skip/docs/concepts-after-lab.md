# Sau lab: List<T> mutation và invariant

`List<T>` là một dãy liên tiếp theo chỉ số. `RemoveAt(i)` xóa phần tử ở `i` và dồn các phần tử còn lại về phía trước; các chỉ số sau điểm xóa không còn trỏ tới cùng phần tử như trước. Trong vòng `for`, điều kiện duyệt phải phù hợp với trạng thái mới của collection.

Invariant hữu ích: sau mỗi bước, mọi phần tử ở vùng đã xử lý phải thỏa eligibility và không phần tử nào ở vùng chưa xử lý bị bỏ qua. Một thuật toán chỉ đúng với trường hợp đơn lẻ chưa đủ để chứng minh invariant.

`List<T>.RemoveAll` thực hiện compact các phần tử cần giữ, thường O(n), ổn định về thứ tự; `RemoveAt` nhiều lần trong vòng lặp có thể tạo nhiều lần dịch chuyển, tối đa O(n²). Đánh giá độ phức tạp trên tập dữ liệu lớn, nhưng ưu tiên chứng minh correctness trước.

Tài liệu chính thức: [List<T>.RemoveAll](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.removeall) và [List<T>.RemoveAt](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.removeat).

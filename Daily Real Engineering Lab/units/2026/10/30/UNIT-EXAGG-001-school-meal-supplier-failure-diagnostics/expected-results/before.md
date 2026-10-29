# Before — starter chưa sửa
- Fixture bình thường: `published=True`, `total=7`, `diagnostics=0`.
- Fixture hai nguồn faulted: `published=False`, nhưng chỉ một diagnostic được giữ lại; fixture `pass=False`.
- Fixture hủy: thay vì phát ra `OperationCanceledException`, hàm trả `ReportOutcome`; fixture `pass=False`.
- `reproduce.ps1` exit 0 khi nhận diện các triệu chứng; `verify.ps1` exit 1.
- Thứ tự diagnostic đầu tiên không phải một hợp đồng cần dựa vào.

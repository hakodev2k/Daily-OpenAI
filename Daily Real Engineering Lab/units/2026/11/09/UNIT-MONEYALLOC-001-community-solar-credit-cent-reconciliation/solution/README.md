# Reference Solution — spoiler

Nguyên nhân: làm tròn độc lập từng phần phân bổ khiến tổng sau làm tròn không còn bằng settlement gốc. Đơn vị hạch toán phải là integer minor units (cent). Tính quota chính xác, lấy phần nguyên bằng floor, sau đó phân phối số cent còn dư theo fractional remainder lớn nhất; khi bằng nhau dùng household ID ordinal để kết quả deterministic.

## Kiểm tra
`dotnet run --project solution/CreditLab.csproj` phải PASS. `verify.ps1` cố ý chỉ chạy starter để xác thực bài làm, không chạy solution.

## Wrong fixes
- Tăng precision của decimal: không bảo toàn tổng sau khi chi trả ở đơn vị cent.
- Dồn mọi chênh lệch vào household cuối: kết quả phụ thuộc thứ tự, gây bất công.
- Dùng double: thêm binary floating-point noise mà không giải quyết bài toán phân bổ đơn vị nhỏ nhất.

## Trade-offs
Largest remainder bảo toàn tổng, deterministic và sai số mỗi phần dưới 1 cent; vẫn cần policy về tie-breaking, negative settlements/refunds và giới hạn overflow trong production.

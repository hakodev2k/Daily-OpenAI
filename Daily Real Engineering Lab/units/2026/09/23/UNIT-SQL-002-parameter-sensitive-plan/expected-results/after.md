# After
- Kết quả nghiệp vụ không thay đổi.
- Execution strategy không còn phụ thuộc một cách nguy hiểm vào tenant tình cờ compile đầu tiên.
- Cả tenant nhỏ và tenant lớn có plan/cardinality behavior phù hợp hơn với shape của chúng.
- Nếu dùng recompilation, bạn phải nêu compile overhead. Nếu dùng SQL Server 2022 PSP, bạn phải chứng minh database compatibility level và plan variants phù hợp. Nếu tách query shape, bạn phải giải thích maintenance cost.
# Incident timeline (synthetic)

- 09:10:00 Alice GET manifest revision=1
- 09:10:02 Bob GET manifest revision=1
- 09:10:18 Alice PUT manifest → 200
- 09:10:23 Bob PUT manifest → 200
- 09:10:30 Alice refresh → ghi chú trở lại giá trị cũ
- API p95: 12ms; CPU: 9%; no 5xx; không có exception. Các chỉ số này không chứng minh không có lost update.

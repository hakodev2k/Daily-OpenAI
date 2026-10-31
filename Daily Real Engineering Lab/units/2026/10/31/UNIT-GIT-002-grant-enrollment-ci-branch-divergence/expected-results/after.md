# After — tiêu chí nghiệm thu

- case-a: chỉ `src/Enrollment/EnrollmentService.cs`.
- case-b: chỉ `src/Api/Approval.cs`.
- case-c: không có file.
- `verify.ps1` PASS ba case và gọi `starter/change-detection.ps1`, không gọi solution.
- Không chỉnh fixture để làm test pass; thay đổi logic change detection.
- Khi áp dụng CI thật, xử lý shallow history/không có common ancestor.

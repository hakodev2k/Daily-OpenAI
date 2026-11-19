# Hint 3
Xem xét expand/contract: thêm cột nullable, deploy code tương thích, dual-write hoặc read-fallback, backfill theo batch có điều kiện chống overwrite, enforce sau khi đã đo null/duplicate và hết old writer. Index unique nên nằm ở bảng Borrower identity thay vì Loan.BorrowerKey nếu mỗi người có nhiều Loan.

# References — Đọc sau khi tự thử

- RFC 9110, HTTP Conditional Requests — If-Match: https://www.rfc-editor.org/rfc/rfc9110#section-13.1.1
- RFC 9110, 412 Precondition Failed: https://www.rfc-editor.org/rfc/rfc9110#section-15.5.13
- RFC 6585, 428 Precondition Required: https://www.rfc-editor.org/rfc/rfc6585#section-3

Đây là local simulation của hợp đồng HTTP, không phải implementation đầy đủ parser ETag theo RFC (ví dụ danh sách ETag, `*`, weak comparison, precedence). Trong production cần dùng parser/framework và quy tắc HTTP tương thích RFC.

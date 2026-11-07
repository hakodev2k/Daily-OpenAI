# After — contract
`verify.ps1` phải PASS trên `starter/` đã sửa; không dùng reference để verify.

- `B-ok`, `C-ok`, `D-ok` đều được nhận ở cycle 1.
- `A-poison` vào DLQ cycle 4; `A-next` chỉ được nhận ở cycle 5 (giữ thứ tự trong charger A).
- Các charger cùng operator có session khác nhau; cùng charger nhưng khác operator cũng khác session.
- Không mất receipt và không xử lý trùng. `reproduce.ps1` có thể FAIL sau khi sửa vì nó kiểm tra triệu chứng cũ — đó là đúng mục đích.

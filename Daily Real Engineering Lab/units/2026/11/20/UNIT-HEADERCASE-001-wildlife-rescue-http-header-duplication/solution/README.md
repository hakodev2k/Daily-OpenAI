# Spoiler — Reference solution
`StringValues.ToString()` gộp các giá trị thành chuỗi phân cách bằng dấu phẩy; adapter chọn token đầu tiên và bỏ mất multiplicity. `solution/Program.cs` kiểm tra `Count == 1` trước khi validate exact zone token. Copy `Router.Route` vào starter rồi chạy `./verify.ps1`; script verify luôn chạy starter, không chạy solution.

Wrong fixes: chọn `FirstOrDefault`, lấy token cuối, `Distinct()` để chấp nhận duplicate-identical — đều che mất contract violation. Trade-off: chấp nhận duplicate-identical có thể hợp lệ nếu các bên thống nhất, nhưng bài này yêu cầu single-value. Trong production trả HTTP 400 và log số giá trị, không log header nhạy cảm. Test thêm comma trong một giá trị, whitespace và casing.

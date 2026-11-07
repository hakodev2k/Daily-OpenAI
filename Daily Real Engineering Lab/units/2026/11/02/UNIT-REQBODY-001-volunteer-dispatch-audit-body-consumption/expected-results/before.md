# Before
Chạy \`./reproduce.ps1\`: exit code 0 khi **tái hiện lỗi**. JSON hợp lệ và JSON malformed đều trả HTTP 400; audit có 2 bản ghi với độ dài khác 0. \`./verify.ps1\` phải exit code khác 0 trên starter chưa sửa. Nếu không tái hiện, kiểm tra SDK và bảo đảm không chạy nhầm \`solution/Program.cs\`.

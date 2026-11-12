# Before
`reproduce.ps1` trả exit 0 khi xác nhận starter in `COOKIE_CONTRACT=FAIL` và tiến trình .NET trả exit 2. Đây là kiểm tra *failure được tái hiện*, không phải kiểm tra fix thành công.

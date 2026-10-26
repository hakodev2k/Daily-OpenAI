# Before
- `run.ps1` smoke direct trả 200.
- `reproduce.ps1` trả exit 0 và log `gateway_status=401; secure_requests=1; authorized=0`.
- `verify.ps1` trả exit khác 0 vì redirected call không đạt hợp đồng. Không sửa harness để biến test thành xanh.
- Nếu môi trường không bind được 127.0.0.1:0, kiểm tra chính sách loopback và .NET 8 runtime.

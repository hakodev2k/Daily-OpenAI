# Before — starter
`./scripts/reproduce.ps1` trả exit 0, in `OBSERVED_FAILURE`. Hai submission có thể bị đánh dấu accepted dù receipt ID rỗng; partner durable receipt count = 0. `./scripts/verify.ps1` trả exit 3.

Nếu Kestrel không bind được loopback, kiểm tra port policy/firewall và .NET 8 SDK. Các con số thời gian không phải assertion.
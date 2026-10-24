# Before — starter nguyên bản
`./scripts/reproduce.ps1` exit **0** khi chứng minh được lỗi: serial đúng cả hai order, overlap có ít nhất một mapping sai. `./scripts/verify.ps1` exit **1** vì không đạt isolation. Không cần độ trễ hay network thật. Nếu SDK thiếu, script có thể lỗi trước khi chạy lab; kiểm tra `dotnet --info`.

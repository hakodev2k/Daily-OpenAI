# Before
./reproduce.ps1 trả 0 với ba PASS: local round-trip đúng, outbound bytes khác partner vector, inbound partner vector decode sai. ./verify.ps1 trả non-zero do contract tests thất bại.

Nếu không reproduce: xác nhận .NET 8 SDK, working directory và starter chưa bị chỉnh.

# Before

- reproduce.ps1 thành công khi phát hiện sai lệch.
- Payload "ALERT" có 5 bytes, outbound sau giải mã có 32 bytes.
- verify.ps1 trả exit code khác 0 với gói ngắn/rỗng.
- Gói đủ 32 bytes có thể pass.
- Marker synthetic 0xD7 trong FixedSizePool làm vấn đề tái hiện ổn định.

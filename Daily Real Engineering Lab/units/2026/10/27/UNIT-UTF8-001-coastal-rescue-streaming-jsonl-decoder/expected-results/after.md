# After
- `./verify.ps1` exit 0 trên learner-modified `starter/`.
- Single read, split inside UTF-8, one byte per read và split near end đều bảo toàn hai record và JSON hợp lệ.
- UTF-8 bị cắt dở tại EOF gây lỗi decoding rõ ràng, không bị chấp nhận âm thầm.
- `./reproduce.ps1` vẫn tái hiện baseline; reference có thể kiểm bằng `dotnet run --project solution/Utf8StreamLab.csproj -- --verify`.

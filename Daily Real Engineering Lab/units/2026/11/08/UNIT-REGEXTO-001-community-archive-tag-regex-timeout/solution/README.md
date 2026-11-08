# Spoiler — Reference solution
Pattern `^(a+)+$` gây catastrophic backtracking trên input dài không khớp. Thay bằng `\A[a-z]{3,32}\z` với `RegexOptions.NonBacktracking`, giữ timeout defense-in-depth.

## Wrong fixes
Tăng timeout chỉ kéo dài thời gian chiếm worker; catch rồi trả true làm sai integrity; chặn riêng độ dài 4001 không sửa độ phức tạp.

## Trade-offs
NonBacktracking không hỗ trợ mọi regex feature (ví dụ backreferences). Với rule đơn giản, vòng lặp ký tự + length check cũng dễ audit. Kiểm thử input biên và malformed, không chỉ happy path.

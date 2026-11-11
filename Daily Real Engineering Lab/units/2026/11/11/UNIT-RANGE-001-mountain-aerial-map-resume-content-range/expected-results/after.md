# After
Sau khi sửa learner-owned `starter/Program.cs`, `./scripts/verify.ps1` phải exit 0: range đúng được ghép thành `ABCDEFGHIJKL`; range sai bị từ chối với `InvalidDataException`. `reproduce.ps1` kiểm tra riêng starter gốc; sau khi sửa, reproduction có thể fail vì symptom đã được loại bỏ — đây là hành vi mong đợi.

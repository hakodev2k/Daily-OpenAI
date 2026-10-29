# Baseline

Chạy `./reproduce.ps1` trên starter nguyên bản: đúng 3 case FAIL là `first-fold-with-offset`, `ambiguous-without-offset`, `ordinary-wrong-offset`; 4 case còn lại PASS; script trả exit code 0 vì đã tái hiện đúng failure set.

Chạy `./verify.ps1` trước khi sửa: trả exit code 1. Nếu baseline khác, kiểm tra bạn đã sửa starter chưa, SDK/runtime, timezone database của hệ điều hành và build output.

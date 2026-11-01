# After — learner fix

`./scripts/verify.ps1` exit 0, `CONTRACT_MISMATCHES=0` cho tất cả bảy trường hợp. Ba replica với 80% không cho phép voluntary eviction; năm replica với 80% cho phép một eviction khi cả năm Ready. Không dùng `reproduce.ps1` để xác nhận fix vì script đó chỉ kiểm tra starter failure.

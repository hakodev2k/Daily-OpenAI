# After

./verify.ps1 phải exit 0:
- slow: returned-before-ready=False; seats=37
- fast: correct=True
- failed-partner: failure-propagated=True
- verification-passed=True

./reproduce.ps1 sau khi fix phải exit khác 0. Harness điều khiển thứ tự bằng Task gate, không phụ thuộc tốc độ máy.

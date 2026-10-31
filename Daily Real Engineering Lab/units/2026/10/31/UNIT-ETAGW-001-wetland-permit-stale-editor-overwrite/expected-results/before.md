# Before
- GET A và GET B cùng ETag "rev-1".
- PUT A và PUT B đều trả 200 dù B vẫn gửi validator cũ.
- Nội dung cuối là của B; ghi chú A bị mất.
- ./reproduce.ps1 in SYMPTOM_REPRODUCED và exit 0.
- ./verify.ps1 in CONTRACT_FAIL và exit khác 0.

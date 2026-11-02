# After — learner-fixed starter
`./scripts/verify.ps1` exit 0, `TOTAL: 4/4`. Snapshot đúng thứ tự `OX-101, OX-102, OX-103, OX-201, OX-301`; TANK-B có hai cảnh báo Severity=5. Gửi lại event giống hệt không tăng số lượng.
`reproduce.ps1` cố ý không còn exit 0 sau khi sửa: reproduce và verify là hai phase khác nhau.

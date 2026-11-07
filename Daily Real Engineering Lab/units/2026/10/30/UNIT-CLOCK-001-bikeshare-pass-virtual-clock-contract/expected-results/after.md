# After — Learner-fixed starter

`verify.ps1`: exit code 0, dòng `PASS: temporal contract at all boundaries.`

Bảng quyết định: `atStart=Allowed`, `beforeExpiry=Allowed`, `atExpiry=Expired`, `beforeStart=NotYetValid`, `invalidWindow=InvalidWindow`, `afterRewind=Allowed`.

`reproduce.ps1` lúc này dự kiến exit code 1 vì triệu chứng ban đầu đã được khắc phục. Không dùng `solution/` làm đầu vào cho `verify.ps1`.

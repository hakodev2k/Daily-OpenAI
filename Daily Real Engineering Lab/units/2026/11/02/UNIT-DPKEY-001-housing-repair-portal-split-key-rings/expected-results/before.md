# Before — starter nguyên bản

- ./run.ps1 trả exit 0 và in báo cáo, không chứng minh correctness.
- local=True, crossReplica=False, restart=True, otherAppRejected=True, tamperRejected=True.
- ./reproduce.ps1 trả exit 0 với REPRODUCED.
- ./verify.ps1 trả exit 1 (PowerShell wrapper ném lỗi) vì crossReplica=False.
- Không có cloud dependency hay secret thực. Nếu không reproduce được, kiểm tra .NET 8 SDK và ASP.NET Core shared framework, chạy lại ./setup.ps1.

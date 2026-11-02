# After — learner-edited starter

- local=True, crossReplica=True, restart=True, otherAppRejected=True, tamperRejected=True.
- ./verify.ps1 trả exit 0 với VERIFIED.
- Ticket không bị lộ ra log, key chỉ nằm trong thư mục tạm của từng lần chạy.
- Chạy thêm reference: dotnet run --project ./solution/DataProtectionLab.csproj -- --verify.
- Khi verify đã đạt, ./reproduce.ps1 có thể trả exit 1 vì triệu chứng starter không còn tồn tại.
- Kiểm thử production cần thêm kiểm tra rotation, encryption at rest, quyền truy cập storage, key loss, clock skew và rollback.

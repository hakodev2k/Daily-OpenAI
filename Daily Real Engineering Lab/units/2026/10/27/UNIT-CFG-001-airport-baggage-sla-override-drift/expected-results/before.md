# Before — starter chưa sửa

- reproduce.ps1 build thành công và xác nhận contract failure có chủ đích.
- EXPECTED_ALERT_MINUTES=5 nhưng EFFECTIVE_ALERT_MINUTES=30.
- EXPECTED_ESCALATION_QUEUE=priority nhưng EFFECTIVE_ESCALATION_QUEUE=standard.
- Exit code của --assert-contract là 2.
- Nếu build fail hoặc exit code khác 2, không tính là đã reproduce; kiểm tra .NET SDK và working directory.

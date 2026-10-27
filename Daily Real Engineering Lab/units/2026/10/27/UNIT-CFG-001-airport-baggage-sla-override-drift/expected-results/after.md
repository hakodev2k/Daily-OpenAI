# After — learner-modified starter

- verify.ps1 chạy trên starter/ đã sửa, không chạy trên solution/.
- Override 5 + priority: EFFECTIVE_ALERT_MINUTES=5, EFFECTIVE_ESCALATION_QUEUE=priority, CONTRACT_PASS.
- Override 11 + priority: EFFECTIVE_ALERT_MINUTES=11, EFFECTIVE_ESCALATION_QUEUE=priority, CONTRACT_PASS.
- Không override: EFFECTIVE_ALERT_MINUTES=30, EFFECTIVE_ESCALATION_QUEUE=standard, CONTRACT_PASS.
- Script phải trả về exit code 0; nếu sai, phải báo lỗi và không được tuyên bố pass.

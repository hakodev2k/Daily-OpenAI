# After

- `./verify.ps1` exit code 0 qua 24 vòng deterministic.
- Case 2 nhân viên: `approved=1, remaining=1`.
- Case 3 nhân viên: `approved=2, remaining=1`.
- Case 1 nhân viên: `approved=0, remaining=1`.
- Các request bị từ chối không làm thay đổi ca trực.
- Cơ chế production phải bảo vệ invariant trên **mọi replica** và có telemetry cho conflict/rejection, không chỉ pass fixture cục bộ.

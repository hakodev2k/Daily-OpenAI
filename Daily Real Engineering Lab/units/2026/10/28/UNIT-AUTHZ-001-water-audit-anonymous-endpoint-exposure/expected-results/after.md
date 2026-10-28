# After (learner-editable starter đã sửa)

`./verify.ps1` exit 0 khi **đồng thời** có: anonymous audit 401, anonymous export 401, authenticated audit 200 và anonymous health 200. Đảm bảo cả hai sensitive routes bị chặn; đừng chỉ sửa một route. Test chạy trên loopback port động, không so sánh latency tuyệt đối.

# After
`./verify.ps1` trên starter đã sửa exit 0: cả 8 cases PASS. V1 weight=12.5 và hazard=NOT_APPLICABLE; v2 giữ đúng weight/hazard; invalid/unknown/ambiguous requests đều bị reject. Không thay đổi test expectations. Production extension: đo reject reason, p95, version distribution và rollback trigger.

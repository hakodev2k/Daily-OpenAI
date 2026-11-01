# Senior challenge — requirement change

Đội SRE bổ sung yêu cầu: trong một giờ cao điểm có thể có một Pod unready, autoscaler không cấp thêm node ngay, và maintenance phải hoàn thành trong 30 phút.

Hãy thiết kế runbook quyết định drain/no-drain theo số replica, currentHealthy, PDB và spare node capacity. Đề xuất tối thiểu hai lựa chọn (`minAvailable` vs `maxUnavailable`), điều kiện abort, quyền phê duyệt, rollback, kiểm thử trong staging và rủi ro unplanned failure. Không được mặc định xóa PDB.

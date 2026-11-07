# Before — starter chưa sửa

- ./reproduce.ps1 trả exit 0 vì quan sát đúng lỗi dự kiến.
- queued_cancelled=True và cancelled_work_executed=False.
- third_entered_before_first_released=True vi phạm giới hạn GPU một job.
- third_fault=SemaphoreFullException; healthy queue và hủy trong callback vẫn có thể pass.
- ./verify.ps1 trả exit 1. Thời gian không phải tiêu chí pass/fail.

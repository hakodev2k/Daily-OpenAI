# Expected before — starter chưa sửa

`./reproduce.ps1` trả exit code **0** khi chứng minh được lỗi. Dấu hiệu chính:

```text
FirstOutcome=none
FirstError=SemaphoreFullException
SecondOutcome=Executed
SecondWorkRan=True
MaxConcurrentWork=2
REPRODUCED: exclusive admission violated.
```

Các dòng `ThirdOutcome`, `WorkFailurePropagated`, `PermitRestoredAfterFailure`, `CancellationObserved` giúp phân biệt hư hỏng admission với đường xử lý lỗi khác. `./verify.ps1` phải trả exit code khác 0 khi starter chưa sửa.

Nếu không reproduce: kiểm tra đang chạy project `src/LabHarness` chứ không phải `solution/Reference.csproj`; kiểm tra chưa sửa starter; chạy lại `./setup.ps1`. Không cần tăng số luồng hay thay đổi thời gian sleep.

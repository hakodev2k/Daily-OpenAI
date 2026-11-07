# Expected after — learner đã sửa starter

`./verify.ps1` trả exit code **0** khi toàn bộ invariant đúng:

```text
FirstOutcome=Executed
FirstError=none
SecondOutcome=Busy
SecondError=none
SecondWorkRan=False
MaxConcurrentWork=1
ThirdOutcome=Executed
WorkFailurePropagated=True
PermitRestoredAfterFailure=True
CancellationObserved=True
PASS: admission and failure-path invariants hold.
```

`./reproduce.ps1` khi chạy trên starter đã sửa sẽ không còn xác nhận lỗi (exit code khác 0): đây là hành vi mong đợi. Nếu `verify` fail, so sánh ownership khi timeout và khi callback ném exception.

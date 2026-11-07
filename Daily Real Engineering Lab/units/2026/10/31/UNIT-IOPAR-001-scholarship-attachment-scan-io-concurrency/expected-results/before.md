# Before
- CAPACITY_8=FAIL: completed=24, throttled=0, allClean=true, peak không quá 2.
- CAPACITY_1 có thể FAIL do throttling nếu máy có ít nhất 2 logical processors.
- run.ps1 trả exit code 1; reproduce.ps1 trả 0 khi bắt đúng triệu chứng.
- elapsedMs chỉ để quan sát xu hướng.

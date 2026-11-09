Rà soát phạm vi `try/finally` quanh `SemaphoreSlim.WaitAsync`. Chỉ gọi `Release` khi lời gọi wait đã acquire permit.

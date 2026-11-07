# After
`CALLS 1`, `CACHE 1`, `SAME True`, `FAILURE_RETRY True`. Gateway chỉ thực thi một lần trong process, lỗi tạm thời vẫn retry được. Không suy diễn thành distributed exactly-once.

# Evidence
09:03:59: committed IDs 101–104. 09:04:00: GET page=1,size=2 → [104,103]. 09:04:01: insert 105 committed. 09:04:02: GET page=2,size=2 → [103,102]. HTTP 200 cả hai lần. Không có deadlock. Điều tra các giả thuyết: cache, thứ tự, transaction isolation, concurrent writes.

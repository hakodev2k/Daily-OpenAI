# Before

- `./reproduce.ps1` chạy baseline và kỳ vọng exit code 0, `REPRODUCED`.
- `AllWorkersShareCachedInstance=True`, `AllResultsMatchKey=True`.
- `FirstKeyCompileCount>1` dưới cold-cache contention; số lần thực tế phụ thuộc scheduling.
- Nếu máy bị giới hạn scheduler và chưa tái hiện, chạy lại; không đánh giá bằng thời gian tuyệt đối.

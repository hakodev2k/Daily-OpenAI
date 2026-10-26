# After — learner-edited starter

- `./verify.ps1` trả exit code 0 và `summary=7/7`.
- Graph hợp lệ lên kế hoạch đủ 6 bước, mỗi dependency đứng trước consumer, kết quả ổn định.
- Graph có cycle, self-cycle, unknown prerequisite hoặc duplicate ID bị từ chối với diagnostic rõ ràng và `Order` rỗng; không công bố partial plan như thành công.
- Graph rỗng hợp lệ.
- `./reproduce.ps1` vẫn tái hiện baseline độc lập với starter, để giữ bằng chứng before/after.
- Có thể tự đối chiếu bằng `dotnet run --project solution/Solution.csproj -- verify`.

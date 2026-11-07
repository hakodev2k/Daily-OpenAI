# Hint 02

Theo dõi từng `CancellationToken` từ request boundary đến `SemaphoreSlim.WaitAsync` và downstream `BuildAsync`. Token nào thực sự được observe ở mỗi bước?

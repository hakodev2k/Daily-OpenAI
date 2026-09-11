# Hint 2

Theo dõi token từ `WithCancellation(...)` vào `GetAsyncEnumerator(CancellationToken)` và sau đó vào thân async iterator. Với async iterator có parameter `CancellationToken`, compiler cần biết token nào đại diện cho cancellation của enumeration.
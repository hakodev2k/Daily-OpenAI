# Hint 3

Khi timeout thắng race, hãy signal `CancellationTokenSource` của attempt và quan sát/await task còn lại trước khi rời timeout boundary. Giữ nguyên contract trả `TimeoutException` cho caller.

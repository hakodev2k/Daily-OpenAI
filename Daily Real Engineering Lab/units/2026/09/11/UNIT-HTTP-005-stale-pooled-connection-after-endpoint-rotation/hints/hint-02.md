# Hint 2

Inspect `SocketsHttpHandler` settings that control how long a pooled connection may remain eligible for reuse. The fix should preserve long-lived `HttpClient` usage.
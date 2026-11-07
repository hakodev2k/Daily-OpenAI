# Hint 3

Look at `SocketsHttpHandler.PooledConnectionLifetime`. A finite lifetime can force periodic reconnection, causing the next connection attempt to observe the current endpoint registry value without recreating `HttpClient` per request.
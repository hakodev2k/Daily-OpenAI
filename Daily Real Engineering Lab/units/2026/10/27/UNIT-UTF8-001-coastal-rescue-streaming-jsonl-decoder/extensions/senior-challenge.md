# Senior extension
Thiết kế `IAsyncEnumerable<ReadOnlyMemory<byte>>` streaming với `CancellationToken`, max line 16 KiB và rejection khi vượt ngưỡng. So sánh `Decoder` + `StringBuilder`, `StreamReader` và `System.IO.Pipelines` về memory bound, backpressure, BOM và observability. Viết test chia frame tại mọi byte offset.

Với `System.Threading.Channels`, xem `BoundedChannelFullMode` và thiết kế producer/consumer chạy đồng thời. `WriteAsync` chỉ có thể backpressure đúng khi chính sách cho phép chờ.

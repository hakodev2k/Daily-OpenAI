# After
Sau fix hợp đồng cancellation, `verify.ps1` exit 0 và in:

    normal_stream_count=3
    direct_parameter_canceled=True
    subscriber_canceled_after_first=True
    subscriber_canceled_before_first=True

`reproduce.ps1` lúc này có thể exit 1 vì lỗi đã được sửa.

# Senior extension — timeout đa node
Worker có thể restart giữa batch; hai node có thể đồng thời tin mình giữ lease. Thiết kế tách (a) elapsed watchdog cục bộ, (b) shared lease expiry, (c) fencing token. Phân tích clock skew, duplicate execution, process restart và source of authority. Không persist raw `GetTimestamp()`.

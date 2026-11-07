# Before — starter
- scripts/reproduce.ps1 trả exit code 0 **khi** nhận diện ObjectDisposedException trên thao tác đọc byte đầu tiên.
- Fake handler đã trả HTTP 200 và payload CSV hợp lệ.
- TrackedStream.IsDisposed = true trước khi caller đọc.
- scripts/verify.ps1 trả exit code khác 0 vì stream không còn đọc được.
- Không có network call thực.

# Reference Solution — chỉ xem sau khi tự reproduce và fix

1. **Symptoms:** Bản lịch sử bị thay đổi khi input hoặc dữ liệu trả về bị sửa; các contract của harness tái hiện điều này.
2. **Evidence:** `Publish` tạo collection ngoài mới, nhưng phần tử `SurveyReading` và `Notes` vẫn chung reference; `GetHistory` trả object nội bộ.
3. **Root cause:** Shallow copy tại ingress và mutable aliasing tại egress. `IReadOnlyList` không làm immutable object bên trong.
4. **Why fix works:** Copy độc lập toàn bộ mutable object graph khi publish và khi trả history. Strings immutable nên có thể reuse.
5. **Verify:** Chỉ sửa `starter/SnapshotStore.cs`, chạy `./scripts/verify.ps1` và yêu cầu cả bốn PASS. Có thể copy `solution/SnapshotStore.cs` sang starter để tự kiểm chứng **sau khi làm bài**.
6. **Alternatives:** Immutable DTO + immutable nested collections; serialized append-only snapshot; database versioned records. Chọn theo ownership, scale và persistence.
7. **Wrong fixes:** `ToList()` chỉ copy outer collection; `AsReadOnly()` chỉ chặn sửa danh sách ngoài; lock không giải quyết aliasing tuần tự; clone chỉ một boundary là chưa đủ.
8. **Production implications:** Lịch sử sai gây lỗi audit, reconciliation, export. Hệ thống thật cần persistence, versioning, access control và transaction boundary.
9. **Trade-offs:** Deep copy có chi phí O(n) theo graph mỗi lần publish/read. Immutable model đổi API nhưng giảm rủi ro. Với graph lớn, cân nhắc snapshot serialization hoặc copy-on-write có ownership rõ.
10. **Senior engineer notices:** Kiểm tra nested mutability, cả hai trust boundary, regression theo hành vi và giới hạn scale; không nhầm read-only interface với immutable state.

Tài liệu chính thức: [Object.MemberwiseClone](https://learn.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone), [List<T>.AsReadOnly](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.asreadonly), [Immutable collections](https://learn.microsoft.com/en-us/dotnet/api/system.collections.immutable).

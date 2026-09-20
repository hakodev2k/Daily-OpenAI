# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Input cuối cùng là `anna`, nhưng request `ann` chậm hơn hoàn thành sau cùng và UI render kết quả `ann`.

## 2. Evidence
Timeline cho thấy intent order là `ann → anna`, còn completion order là `anna → ann`. Starter commit mọi response vào cùng `rendered` state.

## 3. Root cause
Async completion được dùng như quyền commit UI state. Một response cũ vẫn được phép ghi đè state sau khi user intent đã chuyển sang query mới.

## 4. Why the fix works
Gắn generation cho mỗi intent và chỉ commit response nếu generation đó vẫn là current generation. Ví dụ:
```js
let generation = 0;
async function onQueryChanged(query) {
  currentQuery = query;
  const mine = ++generation;
  const response = await fakeSearch(query);
  if (mine !== generation) return;
  rendered = response;
}
```
Điều này biến “request hoàn thành” thành điều kiện cần nhưng chưa đủ; response còn phải thuộc intent hiện hành.

## 5. How to verify
Chạy `./verify.ps1`. Đảo latency hoặc thêm ba query liên tiếp để kiểm tra invariant: final render phải đại diện latest intent, không phải latest completion.

## 6. Alternative fixes
`AbortController` có thể cancel fetch cũ và tiết kiệm work. Một state-management layer có request identity cũng phù hợp. Với server-side search, debounce có thể giảm request volume nhưng không tự nó chứng minh ordering correctness.

## 7. Wrong or misleading fixes
Tăng debounce chỉ làm race ít xuất hiện hơn. Làm request mới “nhanh hơn” không bảo đảm network completion order. Disable input cho tới khi request xong thay đổi UX và thường không phù hợp live search.

## 8. Production implications
Cùng failure pattern có thể làm UI hiển thị stale pricing, stale permissions, hoặc detail của entity trước đó. Với mutation, hậu quả còn nghiêm trọng hơn và cần idempotency/concurrency semantics phía server.

## 9. Trade-offs
Generation check đơn giản và không cần transport cancellation nhưng request cũ vẫn tiêu tài nguyên. Cancellation giảm waste nhưng cần xử lý abort như một expected control path. Hai kỹ thuật có thể kết hợp.

## 10. What a Senior engineer should notice
Đây không chỉ là Promise syntax. Vấn đề là ownership của state transition dưới concurrency: response nào còn có authority để commit? Fix tốt làm invariant đó explicit và test được.
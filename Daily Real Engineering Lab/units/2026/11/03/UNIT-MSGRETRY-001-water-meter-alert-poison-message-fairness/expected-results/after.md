# After
`./verify.ps1` trên learner-edited `starter/Program.cs` trả exit code 0:
- Primary: delivered theo thứ tự `A2,B1,T1,C2`, dead-letter `P1`; P1 được thử 1 lần, T1 2 lần.
- Exhaustion/fairness: `Y1` được giao; `X1` dead-letter sau đúng 3 attempts.
- Healthy-only: `Z1` được giao đúng một lần.

Cần giải thích tại sao ordering trong mỗi site vẫn giữ, và vì sao hệ thống vẫn có thể xử lý site độc lập. Không chấp nhận chỉ tăng tick budget để đạt trạng thái mong muốn.

# After
`./verify.ps1` exit 0 trên starter đã sửa: cả 8 invoice, grouping invariant và null input đều pass. Bốn charge 0.335 và một charge 1.34 cho cùng settlement 1.34. Raw events không thay đổi. `./reproduce.ps1` không còn exit 0. Có thể tự check reference: `dotnet run --project solution/BillingReconciliation.Reference.csproj -- verify`.

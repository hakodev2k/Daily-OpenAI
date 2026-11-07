# Incident timeline — synthetic evidence

| Time | Event | Observation |
|---|---|---|
| 09:00 | Partner updates serializer | Schema and shared key unchanged |
| 09:08 | Return receipts rejected | No transport error |
| 09:12 | Request A | Compact JSON, valid signature, accepted |
| 09:13 | Request B | Pretty JSON, partner signs raw payload, rejected |
| 09:15 | Request C | Body changed after signing, rejected |
| 09:17 | Request D | Reordered JSON, freshly signed, accepted |

Run `contract/Program.cs` to collect deterministic local evidence. All keys and payloads are synthetic.

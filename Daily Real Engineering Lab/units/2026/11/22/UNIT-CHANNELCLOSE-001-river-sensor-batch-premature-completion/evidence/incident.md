# Incident evidence (deterministic scenario)
- 3 producers × 4 readings = 12 expected
- Producer A delay 8ms/reading, B 16ms/reading, C 24ms/reading
- Symptom: early completion; missing readings and writer errors
- Noise: consumer processes in memory without network/database dependencies
- Investigate completion ordering before tuning throughput

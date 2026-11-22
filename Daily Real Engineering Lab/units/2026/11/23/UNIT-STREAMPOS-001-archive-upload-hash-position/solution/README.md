# Reference solution

Root cause: `ComputeHashAsync(upload)` consumes the readable stream and leaves its Position at the end. `CopyToAsync` then copies zero bytes. A valid checksum proves what was hashed, not what was stored.

The reference resets `Position=0` before the second consumer and explicitly requires `CanSeek`. For non-seekable network streams, design a one-pass pipeline that hashes while copying, or spool to bounded temporary storage; do not blindly seek. Validate both checksum and byte-for-byte persistence.

Wrong fixes: ignoring the archive length, checking checksum alone, retrying CopyToAsync without repositioning, or loading unbounded uploads into RAM. Trade-off: replayable streams are simple for local tests; production needs size limits and a streaming hash/copy strategy.

Run `./scripts/verify.ps1` after editing starter, or `dotnet run --project ./solution/ArchiveLab.csproj -- verify` to inspect the reference.

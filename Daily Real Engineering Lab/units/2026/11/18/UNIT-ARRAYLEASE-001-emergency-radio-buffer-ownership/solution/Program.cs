// Reference patch for starter/Program.cs, method DeferredTransmitter.Enqueue:
// byte[] owned = buffer.AsSpan(0, count).ToArray();
// _pending.Add(() => Encoding.ASCII.GetString(owned));
// Apply the patch to starter and run verify.ps1; do not replace the starter program with this note.

namespace PermitIntake;

public sealed record UploadCandidate(string FileName, string ContentType, byte[] Bytes);
public sealed record UploadVerdict(bool Accepted, string Code);

public static class UploadGate
{
    private const int MaxBytes = 1_048_576;

    public static UploadVerdict Validate(UploadCandidate candidate)
    {
        if (!candidate.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            return new(false, "EXTENSION");

        if (!string.Equals(candidate.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            return new(false, "MEDIA_TYPE");

        if (candidate.Bytes.Length is 0 or > MaxBytes)
            return new(false, "SIZE");

        return new(true, "ACCEPTED");
    }
}

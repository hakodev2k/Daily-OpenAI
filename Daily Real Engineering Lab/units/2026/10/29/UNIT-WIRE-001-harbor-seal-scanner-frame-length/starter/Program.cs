using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

internal static class Program
{
    private const int MaxPayloadBytes = 256;

    // Test vectors supplied by the partner's wire-protocol contract.
    private static readonly byte[] Ping = { 0x00, 0x04, 0x21, 0x50, 0x49, 0x4E, 0x47 };
    private static readonly byte[] Ack = { 0x00, 0x03, 0x22, 0x41, 0x43, 0x4B };
    private static readonly byte[] Empty = { 0x00, 0x00, 0x23 };
    private static readonly byte[] ShortPayload = { 0x00, 0x05, 0x21, 0x50, 0x49, 0x4E, 0x47 };
    private static readonly byte[] ReversedHeader = { 0x04, 0x00, 0x21, 0x50, 0x49, 0x4E, 0x47 };

    private static int Main(string[] args)
    {
        return args.Length > 0 && args[0] == "reproduce" ? Reproduce()
             : args.Length > 0 && args[0] == "verify" ? Verify()
             : Demo();
    }

    private static Frame Decode(ReadOnlySpan<byte> packet)
    {
        if (packet.Length < 3)
            throw new InvalidDataException("Truncated frame header");

        // Existing parser from the gateway adapter.
        int declaredLength = BinaryPrimitives.ReadUInt16LittleEndian(packet[..2]);
        Console.WriteLine("wire=" + Convert.ToHexString(packet[..2]) +
                          " declared=" + declaredLength +
                          " actual=" + (packet.Length - 3));

        if (declaredLength > MaxPayloadBytes)
            throw new InvalidDataException("Declared payload exceeds gateway limit");

        if (packet.Length != declaredLength + 3)
            throw new InvalidDataException("Frame length mismatch");

        return new Frame(packet[2], Encoding.UTF8.GetString(packet.Slice(3, declaredLength)));
    }

    private static int Demo()
    {
        try
        {
            var frame = Decode(Ping);
            Console.WriteLine("Accepted: " + frame.Text);
        }
        catch (InvalidDataException ex)
        {
            Console.WriteLine("Rejected: " + ex.Message);
        }
        return 0;
    }

    private static int Reproduce()
    {
        try
        {
            Decode(Ping);
            Console.Error.WriteLine("Baseline symptom absent: valid PING was accepted");
            return 1;
        }
        catch (InvalidDataException ex)
        {
            Console.WriteLine("REPRODUCED: valid PING rejected: " + ex.Message);
            return 0;
        }
    }

    private static int Verify()
    {
        try
        {
            ExpectAccepted(Ping, 0x21, "PING");
            ExpectAccepted(Ack, 0x22, "ACK");
            ExpectAccepted(Empty, 0x23, "");
            ExpectRejected(ShortPayload, "truncated payload");
            ExpectRejected(ReversedHeader, "reversed header");
            ExpectRejected(new byte[] { 0x00, 0x01 }, "truncated header");
            Console.WriteLine("PASS: 3 valid frames and 3 invalid frames");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("FAIL: " + ex.Message);
            return 1;
        }
    }

    private static void ExpectAccepted(byte[] packet, byte expectedType, string expectedText)
    {
        Frame actual = Decode(packet);
        if (actual.Type != expectedType || actual.Text != expectedText)
            throw new Exception("Unexpected decoded frame for " + Convert.ToHexString(packet));
    }

    private static void ExpectRejected(byte[] packet, string caseName)
    {
        try
        {
            Decode(packet);
        }
        catch (InvalidDataException)
        {
            return;
        }
        throw new Exception("Invalid frame accepted: " + caseName);
    }

    private readonly record struct Frame(byte Type, string Text);
}

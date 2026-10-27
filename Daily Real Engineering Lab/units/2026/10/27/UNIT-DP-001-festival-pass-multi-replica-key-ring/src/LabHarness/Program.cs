using System.Security.Cryptography;
using FestivalPass;

if (args.Length == 1 && args[0] == "info")
{
    Console.WriteLine("Festival Pass session diagnostic: commands mint/read.");
    Console.WriteLine("Run reproduce.ps1 before editing, then verify.ps1 after editing.");
    return 0;
}

if (args.Length == 3 && args[0] == "mint")
{
    Directory.CreateDirectory(args[1]);
    var provider = KeyRingFactory.Create("replica-a", args[1]);
    var protector = provider.CreateProtector("festival.ticket-session.v1");
    var token = protector.Protect("ticket-042");
    File.WriteAllText(args[2], token);
    Console.WriteLine("MINTED replica=replica-a process=" + Environment.ProcessId);
    return 0;
}

if (args.Length >= 4 && args.Length <= 5 && args[0] == "read")
{
    var replicaId = args[3];
    var purpose = args.Length == 5 ? args[4] : "festival.ticket-session.v1";
    var provider = KeyRingFactory.Create(replicaId, args[1]);
    var protector = provider.CreateProtector(purpose);
    try
    {
        var value = protector.Unprotect(File.ReadAllText(args[2]));
        if (value == "ticket-042")
        {
            Console.WriteLine("ACCEPTED replica=" + replicaId + " process=" + Environment.ProcessId);
            return 0;
        }
        Console.WriteLine("REJECTED unexpected-payload replica=" + replicaId);
        return 21;
    }
    catch (CryptographicException)
    {
        Console.WriteLine("REJECTED cryptographic-validation replica=" + replicaId);
        return 21;
    }
}

Console.Error.WriteLine("Usage: info | mint <keyRingDirectory> <tokenFile> | read <keyRingDirectory> <tokenFile> <replicaId> [purpose]");
return 2;

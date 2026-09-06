using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace CycleMaintenance.Services.Authentication;

public class Argon2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;

    private const int MemorySize = 65536;
    private const int Iterations = 3;
    private const int DegreeOfParallelism = 4;

    private const int Version = 1;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var hash = DeriveHash(
            password,
            salt,
            MemorySize,
            Iterations,
            DegreeOfParallelism);

        return string.Join(
            "$",
            "argon2id",
            $"v={Version}",
            $"m={MemorySize}",
            $"t={Iterations}",
            $"p={DegreeOfParallelism}",
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('$');

        if (parts.Length != 7 || parts[0] != "argon2id")
            return false;

        try
        {
            var version = int.Parse(parts[1].Replace("v=", ""));
            var memory = int.Parse(parts[2].Replace("m=", ""));
            var iterations = int.Parse(parts[3].Replace("t=", ""));
            var parallelism = int.Parse(parts[4].Replace("p=", ""));

            if (version != Version)
                return false;

            var salt = Convert.FromBase64String(parts[5]);
            var storedHash = Convert.FromBase64String(parts[6]);

            var computedHash = DeriveHash(
                password,
                salt,
                memory,
                iterations,
                parallelism);

            return CryptographicOperations.FixedTimeEquals(
                computedHash,
                storedHash);
        }
        catch
        {
            return false;
        }
    }

    private static byte[] DeriveHash(
        string password,
        byte[] salt,
        int memory,
        int iterations,
        int parallelism)
    {
        var argon2 = new Argon2id(
            Encoding.UTF8.GetBytes(password));

        argon2.Salt = salt;
        argon2.MemorySize = memory;
        argon2.Iterations = iterations;
        argon2.DegreeOfParallelism = parallelism;

        return argon2.GetBytes(HashSize);
    }
}
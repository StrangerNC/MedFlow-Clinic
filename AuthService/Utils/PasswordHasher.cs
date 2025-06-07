using System.Security.Cryptography;

namespace AuthService.Utils;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

    public static Tuple<string, string> Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        return new Tuple<string, string>(Convert.ToHexString(hash), Convert.ToHexString(salt));
    }

    public static bool Verify(Tuple<string, string> passwordHash, string password)
    {
        byte[] hash = Convert.FromHexString(passwordHash.Item1);
        byte[] salt = Convert.FromHexString(passwordHash.Item2);
        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }
}
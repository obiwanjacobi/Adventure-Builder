using System.Security.Cryptography;

namespace Jacobi.AdventureBuilder.ApiService.Account;

internal interface ISaltHasher
{
    (HashValue hash, HashValue salt) Hash(string valueToHash);
    HashValue Hash(string valueToHash, byte[] salt);
    HashValue Hash(string valueToHash, HashValue salt);
    bool Verify(string valueToCompare, byte[] salt, byte[] hash);
    bool Verify(string valueToCompare, HashValue salt, HashValue hash);
}

internal sealed class SaltHasher : ISaltHasher
{
    private const int NumberOfIterations = 100_000;
    private static readonly HashAlgorithmName AlgorithmName = HashAlgorithmName.SHA3_512;

    private readonly HashValue _pepper;

    public SaltHasher()
        : this(new HashValue())
    { }

    // pepper is a secret!
    public SaltHasher(HashValue pepper)
    {
        _pepper = pepper;
    }

    public (HashValue hash, HashValue salt) Hash(string valueToHash)
    {
        var salt = HashValue.NewSalt();
        var hash = Hash(valueToHash, salt);
        return (hash, salt);
    }

    public HashValue Hash(string valueToHash, byte[] salt)
    {
        var valSalt = new HashValue(salt);
        return Hash(valueToHash, valSalt);
    }

    public HashValue Hash(string valueToHash, HashValue salt)
    {
        salt.Validate();
        var saltAndPepper = salt + _pepper;
        var hashValue = Rfc2898DeriveBytes.Pbkdf2(valueToHash, saltAndPepper.Value, NumberOfIterations, AlgorithmName, HashValue.HashSizeInBytes);

        return new HashValue(hashValue);
    }

    public bool Verify(string valueToCompare, byte[] salt, byte[] hash)
    {
        var valSalt = new HashValue(salt);
        var valHash = new HashValue(hash);
        return Verify(valueToCompare, valSalt, valHash);
    }

    public bool Verify(string valueToCompare, HashValue salt, HashValue hash)
    {
        salt.Validate();
        hash.Validate();

        var hashToCompare = Hash(valueToCompare, salt);
        return hashToCompare == hash;
    }
}

internal readonly struct HashValue
{
    internal HashValue(byte[] value)
    {
        Value = value;
        Validate();
    }

    public const int HashSizeInBytes = 32;
    public byte[] Value { get; }

    public override bool Equals(object? obj)
    {
        if (obj is HashValue other)
            return Equals(other);

        return false;
    }

    public bool Equals(HashValue hash)
        => Value.SequenceEqual(hash.Value);
    public static bool operator ==(HashValue left, HashValue right)
        => left.Equals(right);
    public static bool operator !=(HashValue left, HashValue right)
        => !left.Equals(right);
    public static HashValue operator +(HashValue left, HashValue right)
    {
        var newValue = left.Value.Concat(right.Value).ToArray();
        return new HashValue(newValue);
    }
    public override int GetHashCode()
        => BitConverter.ToInt32(Value, 0);
    public override string ToString()
        => $"{Convert.ToHexString(Value)}";

    public static HashValue FromString(string hash)
        => new(Convert.FromHexString(hash));
    public static HashValue NewSalt()
        => new(RandomNumberGenerator.GetBytes(HashSizeInBytes));

    internal void Validate()
    {
        if (Value.Length < HashSizeInBytes)
            throw new ArgumentException("Hash value is too small.");
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using ReraCore.Helper;
using ReraModel.Common;

namespace ReraRepo.Services;

// ── Password hashing (PBKDF2 / SHA-256, no extra NuGet needed) ──────────────

public static class PasswordHelper
{
    private const int Iterations = 100_000;
    private const int HashSize   = 32;
    private const int SaltSize   = 16;

    public static string Hash(string password)
    {
        var salt  = RandomNumberGenerator.GetBytes(SaltSize);
        var hash  = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string storedHash)
    {
        var parts = storedHash.Split(':');
        if (parts.Length != 2) return false;
        try
        {
            var salt      = Convert.FromBase64String(parts[0]);
            var expected  = Convert.FromBase64String(parts[1]);
            var actual    = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }
        catch { return false; }
    }
}

// ── JWT generation / API-key validation ─────────────────────────────────────

public class AuthService
{

    public string GenerateToken(AppUser user)
    {
        var key     = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(ConfigurationHelper.JwtSecret));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var handler = new JwtSecurityTokenHandler();
        var authClaim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.id),
                new Claim(ClaimTypes.Name,           user.username),
                new Claim(ClaimTypes.Role,           user.role),
            };
        var token = new JwtSecurityToken(
                        issuer: ConfigurationHelper.JwtIssuer,
                        audience: ConfigurationHelper.JwtAudience,
                        expires: DateTime.Now.AddDays(1),
                        claims: authClaim,
                        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                        );

        return handler.WriteToken(token);
    }

    public DateTime TokenExpiry() => DateTime.UtcNow.AddHours(8);

    public bool ValidateApiKey(string key)
    {
        var configured = ConfigurationHelper.SE_ApiKey;
        return !string.IsNullOrWhiteSpace(configured) && configured == key;
    }

    public SymmetricSecurityKey SigningKey =>
        new(Encoding.ASCII.GetBytes(ConfigurationHelper.JwtSecret));
}

// ── User store (JSON file, same pattern as ConnectionStore) ──────────────────

public class UserStore
{
    private readonly string _filePath;
    private List<AppUser> _users = new();
    private readonly object _lock = new();

    public UserStore()
    {
        _filePath = Path.Combine(ConfigurationHelper.RootPath, "Data", "users.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        Load();
        EnsureDefaultAdmin();
    }

    private void EnsureDefaultAdmin()
    {
        if (_users.Any(u => u.role == "admin")) return;

        var username = ConfigurationHelper.SE_Username ?? "admin";
        var password = ConfigurationHelper.SE_Password ?? "Admin@123";

        _users.Add(new AppUser
        {
            username     = username,
            passwordHash = PasswordHelper.Hash(password),
            role         = "admin",
            isActive     = true
        });
        Save();
    }

    private void Load()
    {
        lock (_lock)
        {
            if (File.Exists(_filePath))
                _users = JsonSerializer.Deserialize<List<AppUser>>(File.ReadAllText(_filePath)) ?? new();
        }
    }

    private void Save()
    {
        lock (_lock)
            File.WriteAllText(_filePath, JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true }));
    }

    public AppUser? GetByUsername(string username) =>
        _users.FirstOrDefault(u => string.Equals(u.username, username, StringComparison.OrdinalIgnoreCase));

    public AppUser? GetById(string id) => _users.FirstOrDefault(u => u.id == id);

    public void UpdatePassword(string userId, string newHash)
    {
        var user = _users.FirstOrDefault(u => u.id == userId);
        if (user is null) return;
        user.passwordHash = newHash;
        Save();
    }
}

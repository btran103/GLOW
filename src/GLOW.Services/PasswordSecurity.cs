using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace GLOW.Services;

public static class PasswordSecurity
{
    public static (bool IsValid, string ErrorMessage) ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Mật khẩu không được để trống.");
        }

        if (password.Length < 8)
        {
            return (false, "Mật khẩu phải có ít nhất 8 ký tự.");
        }

        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            return (false, "Mật khẩu phải chứa ít nhất 1 chữ cái in hoa (A-Z).");
        }

        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            return (false, "Mật khẩu phải chứa ít nhất 1 chữ cái thường (a-z).");
        }

        if (!Regex.IsMatch(password, @"[0-9]"))
        {
            return (false, "Mật khẩu phải chứa ít nhất 1 chữ số (0-9).");
        }

        // Special characters: !@#$%^&*()_+-=[]{}|;:,.<>?
        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?~`]"))
        {
            return (false, "Mật khẩu bắt buộc phải chứa ít nhất 1 KÝ TỰ ĐẶC BIỆT (ví dụ: @, #, $, %, !, *, &, ?, ...).");
        }

        return (true, string.Empty);
    }

    public static string GenerateSalt()
    {
        byte[] bytes = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public static string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        byte[] combined = Encoding.UTF8.GetBytes(password + salt + "GLOW_SECRET_PEPPER_2026");
        byte[] hash = sha256.ComputeHash(combined);
        return Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string password, string salt, string hash)
    {
        return HashPassword(password, salt) == hash;
    }

    public static string Generate6DigitCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}

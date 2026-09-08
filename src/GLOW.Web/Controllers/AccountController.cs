using System.Security.Claims;
using GLOW.Data;
using GLOW.Data.Entities;
using GLOW.Models;
using GLOW.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

public class AccountController : Controller
{
    private readonly GLOWDbContext _context;
    private readonly IEmailService _emailService;

    public AccountController(GLOWDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // 1. Validate special character requirements in password
        var (isValidPassword, passwordError) = PasswordSecurity.ValidatePassword(model.Password);
        if (!isValidPassword)
        {
            ModelState.AddModelError("Password", passwordError);
            return View(model);
        }

        // 2. Check if Email or Username already exists
        var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower().Trim());
        if (existingEmail != null)
        {
            if (!existingEmail.IsEmailConfirmed)
            {
                // Unconfirmed user re-registering -> resend code
                string newCode = PasswordSecurity.Generate6DigitCode();
                existingEmail.EmailVerificationCode = newCode;
                existingEmail.VerificationCodeExpiresAt = DateTime.Now.AddMinutes(15);
                await _context.SaveChangesAsync();
                await _emailService.SendVerificationCodeAsync(existingEmail.Email, newCode, existingEmail.FullName);
                return RedirectToAction(nameof(VerifyEmail), new { email = existingEmail.Email });
            }
            ModelState.AddModelError("Email", "Email này đã được đăng ký tài khoản.");
            return View(model);
        }

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username.ToLower().Trim());
        if (existingUser != null)
        {
            ModelState.AddModelError("Username", "Tên đăng nhập này đã có người sử dụng.");
            return View(model);
        }

        // 3. Hash password and generate 6-digit OTP code
        string salt = PasswordSecurity.GenerateSalt();
        string hash = PasswordSecurity.HashPassword(model.Password, salt);
        string verificationCode = PasswordSecurity.Generate6DigitCode();

        var newUser = new ApplicationUser
        {
            FullName = model.FullName.Trim(),
            Email = model.Email.Trim().ToLower(),
            Username = model.Username.Trim(),
            PasswordHash = hash,
            Salt = salt,
            IsEmailConfirmed = false,
            EmailVerificationCode = verificationCode,
            VerificationCodeExpiresAt = DateTime.Now.AddMinutes(15),
            CreatedAt = DateTime.Now
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        // 4. Send real confirmation email with OTP code
        await _emailService.SendVerificationCodeAsync(newUser.Email, verificationCode, newUser.FullName);

        TempData["SuccessMessage"] = $"Mã xác nhận 6 số đã được gửi tới email {newUser.Email}. Vui lòng kiểm tra hộp thư (hoặc mục Spam)!";
        if (!_emailService.IsSmtpConfigured())
        {
            TempData["DemoCode"] = verificationCode;
        }

        return RedirectToAction(nameof(VerifyEmail), new { email = newUser.Email });
    }

    [HttpGet]
    public IActionResult VerifyEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return RedirectToAction(nameof(Register));
        }

        return View(new VerifyEmailViewModel { Email = email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower().Trim());
        if (user == null)
        {
            ModelState.AddModelError("", "Không tìm thấy thông tin tài khoản.");
            return View(model);
        }

        if (user.IsEmailConfirmed)
        {
            TempData["SuccessMessage"] = "Email của bạn đã được xác thực trước đó. Vui lòng đăng nhập!";
            return RedirectToAction(nameof(Login));
        }

        if (user.EmailVerificationCode != model.Code.Trim())
        {
            ModelState.AddModelError("Code", "Mã xác nhận 6 số không chính xác.");
            return View(model);
        }

        if (user.VerificationCodeExpiresAt.HasValue && user.VerificationCodeExpiresAt.Value < DateTime.Now)
        {
            ModelState.AddModelError("Code", "Mã xác nhận đã hết hạn (15 phút). Vui lòng bấm gửi lại mã.");
            return View(model);
        }

        // Confirmation successful!
        user.IsEmailConfirmed = true;
        user.EmailVerificationCode = null;
        user.VerificationCodeExpiresAt = null;

        // Also sync profile nickname
        var profile = await _context.UserProfiles.FirstOrDefaultAsync();
        if (profile != null)
        {
            profile.Nickname = user.FullName;
            profile.Username = user.Username;
        }

        await _context.SaveChangesAsync();

        // Sign in user directly
        await SignInUserAsync(user, isPersistent: true);

        TempData["SuccessMessage"] = $"🎉 Chúc mừng {user.FullName}! Tài khoản của bạn đã được kích hoạt thành công!";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendCode(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower().Trim());
        if (user != null && !user.IsEmailConfirmed)
        {
            string newCode = PasswordSecurity.Generate6DigitCode();
            user.EmailVerificationCode = newCode;
            user.VerificationCodeExpiresAt = DateTime.Now.AddMinutes(15);
            await _context.SaveChangesAsync();

            await _emailService.SendVerificationCodeAsync(user.Email, newCode, user.FullName);
            TempData["SuccessMessage"] = $"Đã gửi lại mã xác nhận mới tới {user.Email}!";
            if (!_emailService.IsSmtpConfigured())
            {
                TempData["DemoCode"] = newCode;
            }
        }
        return RedirectToAction(nameof(VerifyEmail), new { email });
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string identifier = model.EmailOrUsername.Trim().ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == identifier || u.Username.ToLower() == identifier);

        if (user == null || !PasswordSecurity.VerifyPassword(model.Password, user.Salt, user.PasswordHash))
        {
            ModelState.AddModelError("", "Email/Tên đăng nhập hoặc mật khẩu không chính xác.");
            return View(model);
        }

        if (!user.IsEmailConfirmed)
        {
            // Send new code & redirect to verify
            string newCode = PasswordSecurity.Generate6DigitCode();
            user.EmailVerificationCode = newCode;
            user.VerificationCodeExpiresAt = DateTime.Now.AddMinutes(15);
            await _context.SaveChangesAsync();
            await _emailService.SendVerificationCodeAsync(user.Email, newCode, user.FullName);

            TempData["WarningMessage"] = "Tài khoản của bạn chưa xác thực Email. Vui lòng nhập mã xác nhận 6 số để kích hoạt!";
            if (!_emailService.IsSmtpConfigured())
            {
                TempData["DemoCode"] = newCode;
            }
            return RedirectToAction(nameof(VerifyEmail), new { email = user.Email });
        }

        // Sign in
        await SignInUserAsync(user, model.RememberMe);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    private async Task SignInUserAsync(ApplicationUser user, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("Username", user.Username),
            new Claim("AvatarUrl", user.AvatarUrl)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = isPersistent,
            ExpiresUtc = isPersistent ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
    }
}

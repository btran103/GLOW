using System.Security.Claims;
using GLOW.Data;
using GLOW.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLOW.Web.Controllers;

[Authorize]
public class AuthController : Controller
{
    private readonly GLOWDbContext _context;
    private readonly IWebHostEnvironment _env;

    public AuthController(GLOWDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Profile()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ApplicationUser? appUser = null;
        if (int.TryParse(userIdStr, out int userId))
        {
            appUser = await _context.Users.FindAsync(userId);
        }

        var profile = await _context.UserProfiles.FirstOrDefaultAsync() ?? new UserProfile();
        var privacy = await _context.PrivacySettings.FirstOrDefaultAsync() ?? new UserPrivacySetting();

        ViewBag.Privacy = privacy;
        ViewBag.AppUser = appUser;
        return View(profile);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(string nickname, string bio, string preferredStyle, int dailyWaterGoalMl, int dailyStudyGoalMinutes, int dailySleepGoalHours)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync();
        if (profile != null)
        {
            profile.Nickname = nickname ?? profile.Nickname;
            profile.Bio = bio ?? profile.Bio;
            profile.PreferredStyle = preferredStyle ?? profile.PreferredStyle;
            profile.DailyWaterGoalMl = dailyWaterGoalMl > 0 ? dailyWaterGoalMl : 2000;
            profile.DailyStudyGoalMinutes = dailyStudyGoalMinutes > 0 ? dailyStudyGoalMinutes : 180;
            profile.DailySleepGoalHours = dailySleepGoalHours > 0 ? dailySleepGoalHours : 8;
            await _context.SaveChangesAsync();
        }

        TempData["SuccessMessage"] = "Đã cập nhật thông tin cá nhân thành công! ✨";
        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
    {
        if (avatarFile == null || avatarFile.Length == 0)
        {
            return Json(new { success = false, message = "Vui lòng chọn một file ảnh." });
        }

        // Validate file size (max 5MB)
        if (avatarFile.Length > 5 * 1024 * 1024)
        {
            return Json(new { success = false, message = "Ảnh không được vượt quá 5MB." });
        }

        // Validate file extension
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var ext = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
        {
            return Json(new { success = false, message = "Chỉ chấp nhận file ảnh: JPG, PNG, GIF, WEBP." });
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out int userId))
        {
            return Json(new { success = false, message = "Không xác định được người dùng." });
        }

        // Save file
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "avatars");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{userId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await avatarFile.CopyToAsync(stream);
        }

        var avatarUrl = $"/uploads/avatars/{fileName}";

        // Update ApplicationUser
        var appUser = await _context.Users.FindAsync(userId);
        if (appUser != null)
        {
            // Delete old local avatar file if exists
            if (!string.IsNullOrEmpty(appUser.AvatarUrl) && appUser.AvatarUrl.StartsWith("/uploads/"))
            {
                var oldPath = Path.Combine(_env.WebRootPath, appUser.AvatarUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            appUser.AvatarUrl = avatarUrl;
        }

        // Update UserProfile
        var profile = await _context.UserProfiles.FirstOrDefaultAsync();
        if (profile != null)
        {
            profile.AvatarUrl = avatarUrl;
        }

        await _context.SaveChangesAsync();

        // Re-sign in to update AvatarUrl claim
        if (appUser != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new Claim(ClaimTypes.Name, appUser.FullName),
                new Claim(ClaimTypes.Email, appUser.Email),
                new Claim("Username", appUser.Username),
                new Claim("AvatarUrl", appUser.AvatarUrl)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30) };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        return Json(new { success = true, avatarUrl = avatarUrl });
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePrivacy(bool locationEnabled, bool allowAiDataAnalysis, bool familyStoriesPrivate, bool cameraHistoryLocalOnly)
    {
        var privacy = await _context.PrivacySettings.FirstOrDefaultAsync();
        if (privacy != null)
        {
            privacy.LocationEnabled = locationEnabled;
            privacy.AllowAiDataAnalysis = allowAiDataAnalysis;
            privacy.FamilyStoriesPrivate = familyStoriesPrivate;
            privacy.CameraHistoryLocalOnly = cameraHistoryLocalOnly;
            privacy.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        TempData["SuccessMessage"] = "Đã cập nhật cài đặt bảo mật! 🔒";
        return RedirectToAction(nameof(Profile));
    }
}

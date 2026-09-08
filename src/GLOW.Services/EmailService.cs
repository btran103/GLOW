using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GLOW.Services;

public interface IEmailService
{
    Task<bool> SendVerificationCodeAsync(string toEmail, string code, string fullName);
    Task<bool> SendResetPasswordCodeAsync(string toEmail, string code, string fullName);
    bool IsSmtpConfigured();
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public bool IsSmtpConfigured()
    {
        var smtpHost = _configuration["Smtp:Host"];
        var smtpUser = _configuration["Smtp:Username"];
        return !string.IsNullOrWhiteSpace(smtpHost) && !string.IsNullOrWhiteSpace(smtpUser);
    }

    public async Task<bool> SendVerificationCodeAsync(string toEmail, string code, string fullName)
    {
        string subject = "🌈 [GLOW] Mã xác nhận đăng ký tài khoản của bạn";
        string htmlBody = $@"
        <div style='font-family: Arial, sans-serif; background-color: #FAF9F6; padding: 30px; border-radius: 20px; max-width: 550px; margin: 0 auto; border: 1px solid #E2E8F0;'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <div style='display: inline-block; width: 50px; height: 50px; border-radius: 50%; background: linear-gradient(135deg, #10B981, #F59E0B, #84CC16); color: white; font-weight: bold; font-size: 20px; line-height: 50px; text-align: center;'>GL</div>
                <h2 style='color: #1E293B; margin-top: 10px;'>Chào mừng bạn đến với GLOW! ✨</h2>
            </div>
            <p style='color: #475569; font-size: 15px;'>Xin chào <strong>{fullName}</strong>,</p>
            <p style='color: #475569; font-size: 15px;'>Cảm ơn bạn đã đăng ký tài khoản trên <strong>GLOW — Personal Life Management</strong>. Dưới đây là mã xác nhận kích hoạt tài khoản của bạn:</p>
            <div style='text-align: center; margin: 25px 0;'>
                <span style='display: inline-block; font-size: 32px; font-weight: 900; letter-spacing: 6px; color: #065F46; background: #A7F3D0; padding: 12px 28px; border-radius: 12px;'>{code}</span>
            </div>
            <p style='color: #64748B; font-size: 13px;'>Mã xác nhận này có hiệu lực trong vòng <strong>15 phút</strong>. Vui lòng không chia sẻ mã này cho bất kỳ ai.</p>
            <hr style='border: none; border-top: 1px solid #E2E8F0; margin: 20px 0;' />
            <p style='color: #94A3B8; font-size: 12px; text-align: center;'>🌈 GLOW — Ghi lại cuộc sống, hiểu chính mình và tỏa sáng mỗi ngày.</p>
        </div>";

        return await SendEmailAsync(toEmail, subject, htmlBody, code);
    }

    public async Task<bool> SendResetPasswordCodeAsync(string toEmail, string code, string fullName)
    {
        string subject = "🔒 [GLOW] Mã khôi phục mật khẩu";
        string htmlBody = $@"
        <div style='font-family: Arial, sans-serif; background-color: #FAF9F6; padding: 30px; border-radius: 20px; max-width: 550px; margin: 0 auto; border: 1px solid #E2E8F0;'>
            <h2 style='color: #1E293B;'>Khôi phục mật khẩu GLOW 🔒</h2>
            <p>Xin chào <strong>{fullName}</strong>,</p>
            <p>Mã xác nhận để đặt lại mật khẩu của bạn là:</p>
            <div style='text-align: center; margin: 20px 0;'>
                <span style='font-size: 28px; font-weight: 800; letter-spacing: 5px; color: #9A3412; background: #FDE68A; padding: 10px 24px; border-radius: 10px;'>{code}</span>
            </div>
            <p style='color: #64748B; font-size: 13px;'>Mã có hiệu lực trong 15 phút.</p>
        </div>";

        return await SendEmailAsync(toEmail, subject, htmlBody, code);
    }

    private async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string code)
    {
        var smtpHost = _configuration["Smtp:Host"];
        var smtpPortStr = _configuration["Smtp:Port"];
        var smtpUser = _configuration["Smtp:Username"];
        var smtpPass = _configuration["Smtp:Password"];
        var fromEmail = _configuration["Smtp:FromEmail"] ?? "glow.noreply@gmail.com";
        var fromName = _configuration["Smtp:FromName"] ?? "GLOW Support";

        _logger.LogInformation("==================================================");
        _logger.LogInformation($"[GLOW EMAIL] Gửi tới: {toEmail} | Mã OTP: {code}");
        _logger.LogInformation("==================================================");

        if (!IsSmtpConfigured())
        {
            _logger.LogWarning("[GLOW EMAIL] SMTP chưa cấu hình. Mã OTP chỉ hiển thị trên trang web để test.");
            return true;
        }

        try
        {
            int port = int.TryParse(smtpPortStr, out int p) ? p : 587;
            using var client = new SmtpClient(smtpHost, port)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
                Timeout = 15000
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation($"[GLOW EMAIL] ✅ Đã gửi email thành công tới {toEmail}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[GLOW EMAIL] ❌ Không thể gửi email tới {toEmail}. Mã OTP: {code}");
            return false;
        }
    }
}

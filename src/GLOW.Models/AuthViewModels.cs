using System.ComponentModel.DataAnnotations;

namespace GLOW.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập Email thực tế của bạn.")]
    [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [MinLength(3, ErrorMessage = "Tên đăng nhập tối thiểu 3 ký tự.")]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập Email hoặc Tên đăng nhập.")]
    [Display(Name = "Email hoặc Tên đăng nhập")]
    public string EmailOrUsername { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ghi nhớ đăng nhập")]
    public bool RememberMe { get; set; } = true;
}

public class VerifyEmailViewModel
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mã xác nhận 6 số.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã xác nhận phải gồm đúng 6 chữ số.")]
    [Display(Name = "Mã xác nhận (OTP)")]
    public string Code { get; set; } = string.Empty;
}
